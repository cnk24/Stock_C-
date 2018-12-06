using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;

namespace Kiwoom
{
    public partial class Api : UserControl
    {
        private static string SCR_NO_SEARCH_CONDITION = "0001";     // 조건검색식 화면번호
        private static string SCR_NO_REQUEST_DATA = "0002";         // 시세데이터 요청
        private static string SCR_NO_ORDER = "0003";                // 주문접수 화면번호
        private static string SCR_NO_WALLET = "0004";               // 지갑정보 화면번호
        private static string SCR_NO_REAL_DATA = "0099";            // 실시간 데이터 화면번호
        private ILog log = null;
        private Queue queue = new Queue();

        private int _ProcDelay = 250;
        private bool _b실시간등록타입 = false;

        public delegate void ConnectionStateHandler(Api sender);
        public delegate void ReceiveTrConditionHandler(Api sender, string[] strCodeList, ConditionInfo info);
        public delegate void ReceiveTrDataHandler(Api sender, TrData data);

        public delegate void ReceiveRealDataHandler(Api sender, RequestData data);
        public delegate void ReceiveChejanDataHandler(Api sender, ChejanData data);

        public delegate void ErrorMessageHandler(string error);

        enum SearchConditionType : int
        {
            Normal = 0,     /// 일반조회(0)
            Realtime = 1    /// 실시간조회(1)
        }

        /// <summary>
        /// 키움API접속성공시 발생하는 이벤트
        /// </summary>
        public event ConnectionStateHandler OnConnected;

        /// <summary>
        /// 사용자조건검색 결과 이벤트
        /// </summary>
        public event ReceiveTrConditionHandler OnReceiveTrCondition;

        /// <summary>
        /// 사용자검색 결과 이벤트
        /// </summary>
        public event ReceiveTrDataHandler OnReceiveTrData;

        /// <summary>
        /// 실시간 수신 이벤트
        /// </summary>
        public event ReceiveRealDataHandler OnReceiveRealData;

        /// <summary>
        /// 체결 결과 이벤트
        /// </summary>
        public event ReceiveChejanDataHandler OnReceiveChejanData;

        /// <summary>
        /// 에러 메세지 이벤트
        /// </summary>
        public event ErrorMessageHandler OnErrorMessage;


        #region [Properties]

        /// <summary>
        /// 키움API연결상태
        /// </summary>
        public bool IsConnected
        {
            get { return m_axKHOpenAPI.GetConnectState() == 1; }
        }

        /// <summary>
        /// 로그인 사용자ID
        /// </summary>
        public string UserID
        {
            get { return m_axKHOpenAPI.GetLoginInfo("USER_ID"); }
        }

        /// <summary>
        /// 로그인 사용자명
        /// </summary>
        public string UserName
        {
            get { return m_axKHOpenAPI.GetLoginInfo("USER_NAME"); }
        }

        /// <summary>
        /// 계좌번호
        /// </summary>
        public string Account
        {
            get;
            set;
        }

        /// <summary>
        /// 계좌목록
        /// </summary>
        public string[] Accounts
        {
            get
            {
                string ret = m_axKHOpenAPI.GetLoginInfo("ACCNO");
                char[] sep = new char[] { ';' };
                return ret.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        #endregion [Properties]


        public Api()
        {
            InitializeComponent();
            log = Log.Get(this.GetType());

            m_axKHOpenAPI.OnEventConnect += M_axKHOpenAPI_OnEventConnect;
            m_axKHOpenAPI.OnReceiveTrData += M_axKHOpenAPI_OnReceiveTrData;
            m_axKHOpenAPI.OnReceiveTrCondition += M_axKHOpenAPI_OnReceiveTrCondition;
            m_axKHOpenAPI.OnReceiveRealCondition += M_axKHOpenAPI_OnReceiveRealCondition;
            m_axKHOpenAPI.OnReceiveMsg += M_axKHOpenAPI_OnReceiveMsg;
            m_axKHOpenAPI.OnReceiveConditionVer += M_axKHOpenAPI_OnReceiveConditionVer;

            m_axKHOpenAPI.OnReceiveRealData += M_axKHOpenAPI_OnReceiveRealData; // 실시간 수신
            m_axKHOpenAPI.OnReceiveChejanData += M_axKHOpenAPI_OnReceiveChejanData; // 체결정보 수신
        }

        private void ErrorEvent(string format, params object[] args)
        {
            string msg = string.Format(format, args);
            if (OnErrorMessage != null)
            {
                OnErrorMessage(msg);
            }
        }

        /// <summary>
        /// << 주문 접수/확인 수신시 이벤트 >>
        /// sGubun – 체결구분
        /// nItemCnt - 아이템갯수
        /// sFidList – 데이터리스트
        /// sGubun – 0:주문체결통보, 1:잔고통보, 3:특이신호
        /// sFidList – 데이터 구분은 ‘;’ 이다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void M_axKHOpenAPI_OnReceiveChejanData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            //axKHOpenAPI.GetChejanData(908);  // 주문/체결시간
            //string 종목코드 = m_axKHOpenAPI.GetChejanData(9001).Replace("A", ""); // 종목코드
            //string 종목명 = m_axKHOpenAPI.GetChejanData(302);  // 종목명
            //axKHOpenAPI.GetChejanData(900);  // 주문수량
            //axKHOpenAPI.GetChejanData(901);  // 주문가격
            //string 체결수량 = m_axKHOpenAPI.GetChejanData(911);  // 체결수량
            //axKHOpenAPI.GetChejanData(910);  // 체결가격

            if (e.sGubun == "0")
            {
                // 주문접수/주문체결

                //주문구분
                //매매구분
                //매도_매수구분
                //주문상태

                if (OnReceiveChejanData != null)
                {
                    ChejanData data = new ChejanData();
                    data.주문상태 = m_axKHOpenAPI.GetChejanData(Common.TypeCode.주문상태.GetHashCode());
                    data.매매구분 = m_axKHOpenAPI.GetChejanData(Common.TypeCode.주문구분.GetHashCode()); // +매수, -매도
                    data.종목코드 = m_axKHOpenAPI.GetChejanData(Common.TypeCode.종목코드.GetHashCode()).Replace("A", "");
                    data.종목명 = m_axKHOpenAPI.GetChejanData(Common.TypeCode.종목명.GetHashCode());
                    data.주문수량 = int.Parse(m_axKHOpenAPI.GetChejanData(Common.TypeCode.주문수량.GetHashCode()));
                    data.주문가 = int.Parse(m_axKHOpenAPI.GetChejanData(Common.TypeCode.주문가.GetHashCode()));
                    data.체결수량 = int.Parse(m_axKHOpenAPI.GetChejanData(Common.TypeCode.체결수량.GetHashCode()));
                    data.체결가 = int.Parse(m_axKHOpenAPI.GetChejanData(Common.TypeCode.체결가.GetHashCode()));

                    OnReceiveChejanData(this, data);
                }
                
                //Logger(Log.검색코드, "주문/체결시간 : " + axKHOpenAPI.GetChejanData(908));
            }
            else if (e.sGubun == "1")
            {
                // 잔고통보
            }
            else if (e.sGubun == "3")
            {
                // 특이신호
            }
        }

        /// <summary>
        /// << 실시간 데이터(OnReceiveRealData) 수신부 >>
        /// 실시간 데이터 수신부 : 장중에만 신호 발생한다. 휴일엔 수신 불가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void M_axKHOpenAPI_OnReceiveRealData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            if (e.sRealType == "주식시세" || e.sRealType == "주식체결" || e.sRealType == "주식예상체결")
            {
                if (OnReceiveRealData != null)
                {
                    RequestData data = new RequestData();
                    data.종목코드 = e.sRealKey.Replace("A", "");
                    data.현재가 = int.Parse(m_axKHOpenAPI.GetCommRealData(e.sRealType, Common.TypeCode.현재가.GetHashCode()));
                    data.거래량 = int.Parse(m_axKHOpenAPI.GetCommRealData(e.sRealType, Common.TypeCode.거래량.GetHashCode()));

                    OnReceiveRealData(this, data);
                }
            }
        }

        private void M_axKHOpenAPI_OnReceiveConditionVer(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveConditionVerEvent e)
        {
            throw new NotImplementedException();
        }

        private void M_axKHOpenAPI_OnReceiveMsg(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {
            log.Debug("msg=" + e.sMsg);
        }

        private void M_axKHOpenAPI_OnReceiveRealCondition(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealConditionEvent e)
        {
            throw new NotImplementedException();
        }

        private void M_axKHOpenAPI_OnReceiveTrCondition(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrConditionEvent e)
        {
            /* OnReceiveTrCondition(LPCTSTR sScrNo, LPCTSTR strCodeList, LPCTSTR strConditionName, int nIndex, int nNext) 
                이벤트 함수로 종목리스트가 들어옵니다. 
                -파라메터 설명 
                sScrNo : 화면번호 
                strCodeList : 조회된 종목리스트(ex:039490;005930;036570;…;) 
                strConditionName : 조회된 조건명 
                nIndex : 조회된 조건명 인덱스 
                nNext : 연속조회 여부(0:연속조회없음, 2:연속조회 있음) */
            if (e.sScrNo == SCR_NO_SEARCH_CONDITION && OnReceiveTrCondition != null)
            {
                string[] codeList = e.strCodeList.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                OnReceiveTrCondition(this, codeList, new ConditionInfo(e.nIndex, e.strConditionName));

                if (e.nNext == 2)
                    m_axKHOpenAPI.SendCondition(e.sScrNo, e.strConditionName, e.nIndex, (int)SearchConditionType.Realtime);
            }
        }

        private void M_axKHOpenAPI_OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            // 예수금상세현황요청 : OPW00001
            if (e.sRQName == "예수금상세현황요청")
            {
                if (OnReceiveTrData != null)
                {
                    TrData data = new TrData();
                    data.RQName = e.sRQName;
                    data.예수금 = Common.f가격변환(m_axKHOpenAPI.GetCommData(e.sTrCode, e.sRQName, 0, "예수금"));

                    OnReceiveTrData(this, data);
                }
            }
            // 계좌평가잔고내역요청 : OPW00018
            else if (e.sRQName == "계좌평가잔고내역요청")
            {
                TrData data = new TrData();
                data.RQName = e.sRQName;
                data.종목번호 = new List<string>();

                int cnt = m_axKHOpenAPI.GetRepeatCnt(e.sTrCode, e.sRQName);

                for (int i = 0; i < cnt; i++)
                {
                    data.종목번호.Add(m_axKHOpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "종목번호").Replace("A", ""));

                    data.종목명.Add(m_axKHOpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "종목명"));
                    data.수익률.Add(Common.changeFormat(m_axKHOpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "수익률(%)")));
                    data.매입가.Add(Common.f가격변환(m_axKHOpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "매입가")));
                    data.보유수량.Add(Common.f가격변환(m_axKHOpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "보유수량")));
                    data.매매가능수량.Add(Common.f가격변환(m_axKHOpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "매매가능수량")));
                    data.현재가.Add(Common.f가격변환(m_axKHOpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "현재가")));
                    data.매입금액.Add(Common.f가격변환(m_axKHOpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "매입금액")));
                    data.수수료합.Add(m_axKHOpenAPI.GetCommData(e.sTrCode, e.sRQName, i, "수수료합"));
                }

                // 연속조회 유무를 판단하는 값 0: 연속(추가조회)데이터 없음, 1:연속(추가조회) 데이터 있음
                int nPrevNext = Int32.Parse(e.sPrevNext);
                if (nPrevNext == 0)
                {
                    if (OnReceiveTrData != null)
                    {
                        OnReceiveTrData(this, data);
                    }
                }
                else
                {
                    GetMyAccountState(nPrevNext);
                }
            }


            //int cCount = m_axKHOpenAPI.GetRepeatCnt(e.sTrCode, e.sRecordName);
            //for (int i = 0; i < cCount; i++)
            //{
            //    string sName = m_axKHOpenAPI.GetCommData(e.sTrCode, e.sRecordName, i, "종목명");
            //    sName = sName.Trim();
            //    string sPrice = m_axKHOpenAPI.GetCommData(e.sTrCode, e.sRecordName, i, "현재가");
            //    sPrice = sPrice.Trim();
            //    log.Debug("[" + i.ToString("00") + "] name=" + sName + ", price=" + sPrice);
            //}
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 키움API 접속
        /// </summary>
        public void Connect()
        {
            Disconnect();
            m_axKHOpenAPI.CommConnect();
        }

        /// <summary>
        /// 키움API 접속종료
        /// </summary>
        public void Disconnect()
        {
            if (IsConnected)
            {
                m_axKHOpenAPI.CommTerminate();
                log.Debug("KHOpenAPI.CommTerminate() Call...");
            }

            EndMicroTime();
        }

        /// <summary>
        /// 사용자가 설정한 조건식 리스트를 가져온다.
        /// </summary>
        /// <returns>조건식리스트</returns>
        public ConditionInfo[] GetConditionInfoList()
        {
            List<ConditionInfo> list = new List<ConditionInfo>();

            // 서버에 저장된 사용자 조건식을 가져온다.
            int ret = m_axKHOpenAPI.GetConditionLoad();
            Debug.Assert(ret != 0);
            if (ret > 0)
            {
                string str = m_axKHOpenAPI.GetConditionNameList();
                log.Debug("수신받은 조건식목록='" + str + "'");
                char[] sep = new char[] { ';' };
                string[] strList = str.Split(sep, StringSplitOptions.RemoveEmptyEntries);

                foreach (string sItem in strList)
                {
                    string[] strKV = sItem.Split('^');
                    Debug.Assert(strKV.Length == 2);

                    ConditionInfo info = new ConditionInfo(Convert.ToInt32(strKV[0]), strKV[1]);
                    list.Add(info);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 조건검색 결과 요청(실시간)
        /// </summary>
        /// <param name="info">요청할 조건검색식 정보</param>
        /// <returns>성공시 true</returns>
        public bool RequestSearchCondition(ConditionInfo info)
        {
            /*  SendCondition
                -반환값 : FALSE(실패), TRUE(성공) 
                -파라메터 설명 
                    strScrNo : 화면번호 
                    strConditionName :GetConditionNameList()로 불러온 조건명중 하나의 조건명. 
                    nIndex : GetCondionNameList()로 불러온 조건인덱스. 
                    nSearch : 일반조회(0), 실시간조회(1), 연속조회(2) 
                        nSearch 를 0으로 조회하면 단순 해당 조건명(식)에 맞는 종목리스트를  
                        받아올 수 있습니다. 1로 조회하면 해당 조건명(식)에 맞는 종목리스트를 받아 
                        오면서 실시간으로 편입, 이탈하는 종목을 받을 수 있는 조건이 됩니다. 
                        -1번으로 조회 할 수 있는 화면 개수는 최대 10개까지 입니다. 
                        -2은 OnReceiveTrCondition 이벤트 함수에서 마지막 파라메터인 nNext가 “2”로 
                        들어오면 종목이 더 있기 때문에 다음 조회를 원할 때 OnReceiveTrCondition 
                        이벤트 함수에서 사용하시면 됩니다. 
                -결과값 
                OnReceiveTrCondition(LPCTSTR sScrNo, LPCTSTR strCodeList, LPCTSTR strConditionName, int nIndex, int nNext) 
                이벤트 함수로 종목리스트가 들어옵니다. 
            */
            return m_axKHOpenAPI.SendCondition(SCR_NO_SEARCH_CONDITION, info.Name, (int)info.Index, (int)SearchConditionType.Realtime) == 1;
        }

        public Common.ErrorCode RequestData(string[] codeList)
        {
            string strCodes = string.Join(";", codeList);
            /*
            LONG CommKwRqData(LPCTSTR sArrCode, BOOL bNext, int nCodeCount, int nTypeFlag, LPCTSTR sRQName, LPCTSTR sScreenNo) 
            설명 복수종목조회 Tran을 서버로 송신한다. 
            입력값 
                sArrCode – 종목리스트 
                bNext – 연속조회요청 
                nCodeCount – 종목개수 
                nTypeFlag – 조회구분 
                sRQName – 사용자구분 명 
                sScreenNo – 화면번호[4] 
            반환값 
                OP_ERR_RQ_STRING – 요청 전문 작성 실패 
                OP_ERR_NONE - 정상처리 
            비고 
                sArrCode – 종목간 구분은 ‘;’이다. nTypeFlag – 0:주식관심종목정보, 3:선물옵션관심종목정보 ex) openApi.CommKwRqData(“000660;005930”, 0, 2, 0, “RQ_1”, “0101”); */
            return (Common.ErrorCode)m_axKHOpenAPI.CommKwRqData(strCodes, 0, codeList.Length, 0, "RQName", SCR_NO_REQUEST_DATA);
        }

        private void M_axKHOpenAPI_OnEventConnect(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            log.Debug("errCode=" + e.nErrCode.ToString());
            Debug.WriteLine("KHOpenAPI_OnEventConnect=" + e.nErrCode.ToString());
            if (OnConnected != null)
                OnConnected(this);

            // 타이머 실행
            SetMicroTime();

            // Task 실행
            var t = new Task(QueueProc);
            t.Start();
        }


        /// <summary>
        /// 실시간 데이터 요청 메서드
        /// 처음 등록시 "0"
        /// 추가 등록시 "1"
        /// </summary>
        public int setRealReg(string str종목코드)
        {
            string str실시간등록타입 = "0";
            if (_b실시간등록타입 == false)
            {
                _b실시간등록타입 = true;
            }
            else
            {
                str실시간등록타입 = "1";
            }

            string strFidList = string.Format("{0};{1}",
                                                Common.TypeCode.현재가.GetHashCode(),
                                                Common.TypeCode.거래량.GetHashCode());

            int nRet = m_axKHOpenAPI.SetRealReg(SCR_NO_REAL_DATA, str종목코드, strFidList, str실시간등록타입);

            if (Error.IsError(nRet))
            {
                ErrorEvent("[{0}] 실시간시세등록 성공", str종목코드);
            }
            else
            {
                ErrorEvent("[{0}] 실시간시세등록 실패 [에러] : {1}", str종목코드, Error.GetErrorMessage());
            }

            return nRet;
        }

        /// <summary>
        /// 실시간 데이터 중지 메서드
        /// </summary>
        public void setRealRemove(string str종목코드)
        {
            m_axKHOpenAPI.SetRealRemove(SCR_NO_REAL_DATA, str종목코드);
        }

        /// <summary>
        /// 해당 화면번호로 설정한 모든 실시간 데이터 요청을 제거합니다.
        /// 화면을 종료할 때 반드시 이 메서드를 호출해야 합니다.
        /// </summary>
        public void disconnectRealData()
        {
            if (IsConnected)
            {
                m_axKHOpenAPI.DisconnectRealData(SCR_NO_REAL_DATA);
            }
        }

        /// <summary>
        /// 매수 접수
        /// </summary>
        public int OrderBuy(string str종목코드, int n주문수량, int n주문가)
        {
            // ======================================================================
            // 거래구분 취득
            // 0:지정가, 3:시장가, 5:조건부지정가, 6:최유리지정가, 7:최우선지정가,
            // 10:지정가IOC, 13:시장가IOC, 16:최유리IOC, 20:지정가FOK, 23:시장가FOK,
            // 26:최유리FOK, 61:장개시전시간외, 62:시간외단일가매매, 81:시간외종가
            // ======================================================================
            string str거래구분 = "00";

            string str원주문번호 = "0";

            int BlRet;
            queue.Enqueue(BlRet = m_axKHOpenAPI.SendOrder("매수주문", SCR_NO_ORDER, Account,
                                                Common.매매구분.신규매수.GetHashCode(), str종목코드, n주문수량,
                                                n주문가, str거래구분, str원주문번호));

            //if (Error.IsError(BlRet))
            //{
            //    ErrorEvent("매수 주문이 전송 되었습니다");
            //}
            //else
            //{
            //    ErrorEvent("매수 주문이 전송 실패 하였습니다. [에러] : " + BlRet);
            //}

            return BlRet;
        }

        /// <summary>
        /// 매도 접수
        /// </summary>
        public void OrderSell(string str종목코드, int n주문수량, int n주문가)
        {
            // ======================================================================
            // 거래구분 취득
            // 0:지정가, 3:시장가, 5:조건부지정가, 6:최유리지정가, 7:최우선지정가,
            // 10:지정가IOC, 13:시장가IOC, 16:최유리IOC, 20:지정가FOK, 23:시장가FOK,
            // 26:최유리FOK, 61:장개시전시간외, 62:시간외단일가매매, 81:시간외종가
            // ======================================================================
            string str거래구분 = "00";

            string str원주문번호 = "0";

            int SlRet;
            queue.Enqueue(SlRet = m_axKHOpenAPI.SendOrder("매도주문", SCR_NO_ORDER, Account,
                                                        Common.매매구분.신규매도.GetHashCode(), str종목코드, n주문수량,
                                                        n주문가, str거래구분, str원주문번호));

            //if (Error.IsError(SlRet))
            //{
            //    ErrorEvent("매도 주문이 전송 되었습니다");
            //}
            //else
            //{
            //    ErrorEvent("매도 주문이 전송 실패 하였습니다. [에러] : " + SlRet);
            //}
        }

        /// <summary>
        /// 주문취소
        /// </summary>
        public int OrderCancel(string str종목코드, string str주문번호, Common.매매구분 e매매구분)
        {
            int nRet;
            queue.Enqueue(nRet = m_axKHOpenAPI.SendOrder("주문취소",
                                                    SCR_NO_ORDER,
                                                    Account,
                                                    e매매구분.GetHashCode(),              // 매매구분 (3:매수취소, 4:매도취소)
                                                    str종목코드,    // 종목코드
                                                    0,      // 주문수량
                                                    0,      // 주문가격 
                                                    "00",           // 거래구분 (00:지정가, 03:시장가)
                                                    str주문번호));           // 원주문 번호

            //if (Error.IsError(nRet))
            //{
            //    ErrorEvent("[{0}] 주문취소 전송 되었습니다", e매매구분.GetHashCode());
            //}
            //else
            //{
            //    ErrorEvent("[{0}] 주문취소 전송 실패 하였습니다. [에러] : " + nRet, e매매구분.GetHashCode());
            //}

            return nRet;
        }

        public void GetWallet()
        {
            // 예수금상세현황요청
            m_axKHOpenAPI.SetInputValue("계좌번호", Account);
            int nRet = m_axKHOpenAPI.CommRqData("예수금상세현황요청", "OPW00001", 0, SCR_NO_WALLET);
            //if (!Error.IsError(nRet))
            //{
            //    ErrorEvent("[예수금상세현황요청] " + Error.GetErrorMessage());
            //}

            // 계좌평가잔고내역요청 - OPW00018 은 한번에 20개의 종목정보를 반환
            GetMyAccountState(0);
        }

        /// <summary>
        /// 20종목이상을 조회하려면 "계좌평가잔고내역" 연속조회를 구현
        /// </summary>
        /// <param name="prevNext"></param>
        /// <returns></returns>
        public void GetMyAccountState(int prevNext)
        {
            // 계좌평가잔고내역요청 - opw00018 은 한번에 20개의 종목정보를 반환
            m_axKHOpenAPI.SetInputValue("계좌번호", Account);
            int nRet = m_axKHOpenAPI.CommRqData("계좌평가잔고내역요청", "OPW00018", prevNext, SCR_NO_WALLET);
            //if (!Error.IsError(nRet))
            //{
            //    ErrorEvent("[계좌평가잔고내역요청] " + Error.GetErrorMessage());
            //}
        }

        //public void 미체결Call()
        //{
        //    Thread.Sleep(5000);

        //    // 매수미체결 요청
        //    axKHOpenAPI.SetInputValue("계좌번호", 계좌번호);
        //    axKHOpenAPI.SetInputValue("체결구분", "1");                           // 체결구분 = 0:전체, 1:종목
        //    axKHOpenAPI.SetInputValue("매매구분", "2");                           // 매매구분 = 0:전체, 1:매도, 2:매수
        //    axKHOpenAPI.CommRqData("매수미체결", "opt10075", 0, "0004");

        //    //매도미체결 요청
        //    axKHOpenAPI.SetInputValue("계좌번호", 계좌번호);
        //    axKHOpenAPI.SetInputValue("체결구분", "1");                           // 체결구분 = 0:전체, 1:종목
        //    axKHOpenAPI.SetInputValue("매매구분", "1");                           // 매매구분 = 0:전체, 1:매도, 2:매수
        //    axKHOpenAPI.CommRqData("매도미체결", "opt10075", 0, "0005");
        //}



        #region [ETC]

        /// <summary>
        /// 250ms 간격으로 큐 내용 실행
        /// </summary>
        private void QueueProc()
        {
            if (_ProcDelay == 0)
            {
                _ProcDelay = 250;
                if (queue.Count > 0)
                {
                    queue.Dequeue();
                }
            }            
        }

        #endregion [ETC]



        #region [1ms Timer Tick]

        //////////////////////////////////////////////////////////////////////////
        // P/Invoke declarations
        private int mTimerId;
        private TimerEventHandler mHandler;
        private delegate void TimerEventHandler(int id, int msg, IntPtr user, int dw1, int dw2);
        private const int TIME_PERIODIC = 1;
        private const int EVENT_TYPE = TIME_PERIODIC;
        [DllImport("winmm.dll")]
        private static extern int timeSetEvent(int delay, int resolution, TimerEventHandler handler, IntPtr user, int eventType);
        [DllImport("winmm.dll")]
        private static extern int timeKillEvent(int id);
        [DllImport("winmm.dll")]
        private static extern int timeBeginPeriod(int msec);
        [DllImport("winmm.dll")]
        private static extern int timeEndPeriod(int msec);
        //////////////////////////////////////////////////////////////////////////

        private void SetMicroTime()
        {
            timeBeginPeriod(1);
            mHandler = new TimerEventHandler(TimerCallback);
            mTimerId = timeSetEvent(1, 0, mHandler, IntPtr.Zero, EVENT_TYPE);
        }

        private void EndMicroTime()
        {
            mTimerId = 0;
            int err = timeKillEvent(mTimerId);
            timeEndPeriod(1); // 1ms
            // Ensure callbacks are drained
            Thread.Sleep(100);
        }

        //private int MonDelay;
        //public int OutSensor_OffCheckDelay;
        private void TimerCallback(int id, int msg, IntPtr user, int dw1, int dw2)
        {
            if (_ProcDelay > 0) _ProcDelay--;
        }

        #endregion [1ms Timer Tick]


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiwoom
{
    public class Common
    {
        public enum ErrorCode : int
        {
            OP_ERR_NONE = 0,                        // 정상처리
            OP_ERR_FAIL = -10,                      // 실패
            OP_ERR_LOGIN = -100,                    // 사용자정보교환실패
            OP_ERR_CONNECT = -101,                  // 서버접속실패
            OP_ERR_VERSION = -102,                  // 버전처리실패
            OP_ERR_FIREWALL = -103,                 // 개인방화벽실패
            OP_ERR_MEMORY = -104,                   // 메모리보호실패
            OP_ERR_INPUT = -105,                    // 함수입력값오류
            OP_ERR_SOCKET_CLOSED = -106,            // 통신연결종료
            OP_ERR_SISE_OVERFLOW = -200,            // 시세조회과부하
            OP_ERR_RQ_STRUCT_FAIL = -201,           // 전문작성초기화실패
            OP_ERR_RQ_STRING_FAIL = -202,           // 전문작성입력값오류
            OP_ERR_NO_DATA = -203,                  // 데이터없음
            OP_ERR_OVER_MAX_DATA = -204,            // 조회가능한종목수초과
            OP_ERR_DATA_RCV_FAIL = -205,            // 데이터수신실패
            OP_ERR_OVER_MAX_FID = -206,             // 조회가능한FID수초과
            OP_ERR_REAL_CANCEL = -207,              // 실시간해제오류
            OP_ERR_ORD_WRONG_INPUT = -300,          // 입력값오류
            OP_ERR_ORD_WRONG_ACCTNO = -301,         // 계좌비밀번호없음
            OP_ERR_OTHER_ACC_USE = -302,            // 타인계좌사용오류
            OP_ERR_MIS_2BILL_EXC = -303,            // 주문가격이20억원을초과
            OP_ERR_MIS_5BILL_EXC = -304,            // 주문가격이50억원을초과
            OP_ERR_MIS_1PER_EXC = -305,             // 주문수량이총발행주수의1%초과오류
            OP_ERR_MIS_3PER_EXC = -306,             // 주문수량이총발행주수의3%초과오류
            OP_ERR_SEND_FAIL = -307,                // 주문전송실패
            OP_ERR_ORD_OVERFLOW = -308,             // 주문전송과부하
            OP_ERR_MIS_300CNT_EXC = -309,           // 주문수량300계약초과
            OP_ERR_MIS_500CNT_EXC = -310,           // 주문수량500계약초과
            OP_ERR_ORD_WRONG_ACCTINFO = -340,       // 계좌정보없음
            OP_ERR_ORD_SYMCODE_EMPTY = -500,        // 종목코드없음
        }

        public enum TypeCode : int
        {
            현재가 = 10,
            거래량 = 15,
            최우선매도호가 = 27,
            최우선매수호가 = 28,
            종목명 = 302,
            상한가 = 305,
            하한가 = 306,
            주문수량 = 900,
            주문가 = 901,
            미체결수량 = 902,
            원주문번호 = 904,
            주문구분 = 905,
            매매구분 = 906,
            주문_체결시간 = 908,
            체결번호 = 909,
            체결가 = 910,
            체결수량 = 911,
            주문상태 = 913,
            보유수량 = 930,
            매입단가 = 931,
            주문가능수량 = 933,
            매도_매수구분 = 946,
            예수금 = 951,
            종목코드 = 9001,
            계좌번호 = 9201,
            주문번호 = 9203
        }

        public enum 매매구분 : int
        {
            신규매수 = 1,
            신규매도 = 2,
            매수취소 = 3,
            매도취소 = 4,
        }


        public static string changeFormat(string data, int percent = 0)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;

            string formatData = string.Empty;

            if (percent == 0)
            {
                int d = Convert.ToInt32(data);
                formatData = string.Format("{0:N0}", Math.Abs(d));
            }
            else if (percent == 1)
            {
                double f = Convert.ToDouble(data) / 100.0;
                formatData = string.Format("{0:F2}", f);
            }
            else if (percent == 2)
            {
                double f = Convert.ToDouble(data);
                formatData = string.Format("{0:F2}", f);
            }

            return formatData;
        }

        public static int f가격변환(string data)
        {
            if (string.IsNullOrEmpty(data)) return 0;

            int d = Convert.ToInt32(data);
            d = Math.Abs(d);
            return d;
        }

        private bool f장시간체크()
        {
            if (DateTime.Now.TimeOfDay < TimeSpan.Parse("09:00:00")
                || DateTime.Now.TimeOfDay > TimeSpan.Parse("15:30:00"))
            {
                return false;
            }

            return true;
        }


    }
}

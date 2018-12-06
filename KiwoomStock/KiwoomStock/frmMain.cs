using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kiwoom;
using log4net;

namespace KiwoomStock
{
    public partial class frmMain : Form
    {
        private Kiwoom.Api m_Kiwoom = null;
        private ILog log = null;
        private PostgreSQL m_pgSQL = new PostgreSQL();
        private Queue queue = new Queue();

        public frmMain()
        {
            InitializeComponent();
            Log.Init();

            log = Log.Get(this.GetType());
            log.Debug("Start...");

            // 키움API객체 생성
            m_Kiwoom = new Kiwoom.Api();
            m_Kiwoom.OnConnected += M_Kiwoom_OnConnected;
            //m_Kiwoom.OnReceiveTrCondition += M_Kiwoom_OnReceiveTrCondition;
            m_Kiwoom.OnReceiveTrData += M_Kiwoom_OnReceiveTrData;
            m_Kiwoom.OnReceiveChejanData += M_Kiwoom_OnReceiveChejanData;
            m_Kiwoom.OnReceiveRealData += M_Kiwoom_OnReceiveRealData;

            m_Kiwoom.OnErrorMessage += M_Kiwoom_OnErrorMessage;
        }


        #region [FORM EVENT]

        private void frmMain_Load(object sender, EventArgs e)
        {
            dgvWallet.Columns.Add("종목번호", "종목번호");
            dgvWallet.Columns.Add("종목명", "종목명");
            dgvWallet.Columns.Add("수익률", "수익률");
            dgvWallet.Columns.Add("매입가", "매입가");
            dgvWallet.Columns.Add("보유수량", "보유수량");
            dgvWallet.Columns.Add("매매가능수량", "매매가능수량");
            dgvWallet.Columns.Add("현재가", "현재가");
            dgvWallet.Columns.Add("매입금액", "매입금액");
            dgvWallet.Columns.Add("수수료합", "수수료합");


            var communicator = new MsgBusCommunicator();
            var stub = new MsgBusStub(communicator);

            communicator.OnMessageEvent += communicator_OnMessageEvent;

            communicator.StartPumpMsgInBackground();
            stub.HandlePythonWorldInvokeForEver();

            // Task 실행
            var t = new Task(QueueProc);
            t.Start();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_Kiwoom.disconnectRealData();
            m_Kiwoom.Disconnect();
            log.Debug("Close...");
            m_Kiwoom.Dispose();
        }

        #endregion [FORM EVENT]


        #region [Communicator Event]

        private void communicator_OnMessageEvent(string msgName, string message)
        {
            debugLog("type:{0} - msg:{1}", msgName, message);
        }

        #endregion [Communicator Event]



        #region [KIWOOM EVENT]

        private void M_Kiwoom_OnErrorMessage(string error)
        {
            debugLog(error);
        }

        private void M_Kiwoom_OnConnected(Api sender)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(@"D:/kiwoomAccount.info");
            m_Kiwoom.Account = file.ReadLine();
            file.Close();

            btnConnect.Checked = true;
            lbMsg.Text = "연결되었습니다!";
        }

        private void M_Kiwoom_OnReceiveTrData(Api sender, TrData data)
        {
            if (data.RQName == "예수금상세현황요청")
            {
                btnWallet.Text = Common.changeFormat(data.예수금.ToString());
            }
            else if (data.RQName == "계좌평가잔고내역요청")
            {
                dgvWallet.Rows.Clear();

                for (int i=0; i<data.종목번호.Count; i++)
                {
                    dgvWallet.Rows.Add(data.종목번호[i], data.종목명[i], data.수익률[i],
                                        data.매입가[i], data.보유수량[i], data.매매가능수량[i],
                                        data.현재가[i], data.매입금액[i], data.수수료합[i]);
                }
            }
        }

        private void M_Kiwoom_OnReceiveChejanData(Api sender, ChejanData data)
        {
            if (data.주문상태.Equals("접수"))
            {
                if (data.매매구분.EndsWith("매수"))
                {

                }
                else if (data.매매구분.EndsWith("매도"))
                {

                }

                orderLog("[접수] [{0}] [{1}:{2}] [가격: {3}] [수량: {4}]",
                        data.매매구분, data.종목코드, data.종목명,
                        data.주문가, data.주문수량);
            }
            else if (data.주문상태.Equals("체결"))
            {
                if (data.매매구분.EndsWith("매수"))
                {

                }
                else if (data.매매구분.EndsWith("매도"))
                {

                }

                // 보유종목 갱신

                orderLog("[체결] [{0}] [{1}:{2}] [가격: {3}] [수량: {4}]",
                        data.매매구분, data.종목코드, data.종목명,
                        data.체결가, data.체결수량);
            }
        }

        private void M_Kiwoom_OnReceiveRealData(Api sender, RequestData data)
        {
            // 실시간 시세 DB 저장
            queue.Enqueue(m_pgSQL.insertStockData(data.종목코드, data.현재가, data.거래량));
        }

        #endregion [KIWOOM EVENT]


        #region [FORM CONTROL EVENT]

        private void btnConnect_CheckedChanged(object sender, EventArgs e)
        {
            if (btnConnect.Checked)
            {
                // DisConnect
                m_Kiwoom.Disconnect();
            }
            else
            {
                // Connect
                btnConnect.Checked = false;
                if (!m_Kiwoom.IsConnected)
                {
                    m_Kiwoom.Connect();
                }
            }
        }

        private void btnWallet_Click(object sender, EventArgs e)
        {
            m_Kiwoom.GetWallet();
        }

        #endregion [FORM CONTROL EVENT]



        #region [ETC]

        /// <summary>
        /// 큐 내용 실행
        /// </summary>
        private void QueueProc()
        {
            if (queue.Count > 0)
            {
                queue.Dequeue();
            }
        }

        /// <summary>
        /// 디버그 로그 출력
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void debugLog(string format, params object[] args)
        {
            this.Invoke(new Action(delegate ()
            {
                string dir= Application.StartupPath + @"\log\로그\";
                DirectoryInfo df = new DirectoryInfo(dir);
                if (!df.Exists)
                {
                    df.Create();
                }
                df = new DirectoryInfo(dir);
                if (!df.Exists)
                {
                    df.Create();
                }

                string message = string.Format(format, args);

                DateTime dt = DateTime.Now;
                message = dt.ToString("[yyyy-MM-dd HH:mm:ss] ") + message;

                string fileName = dt.ToString("yyyy-MM-dd") + ".log";

                logComm.Items.Add(message);
                logComm.SelectedIndex = logComm.Items.Count - 1;
                File.AppendAllText(dir + fileName, message + "\r\n");
            }));
        }

        /// <summary>
        /// 주문관련 로그 출력
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void orderLog(string format, params object[] args)
        {
            this.Invoke(new Action(delegate ()
            {
                string dir = Application.StartupPath + @"\log\주문내역\";
                DirectoryInfo df = new DirectoryInfo(dir);
                if (!df.Exists)
                {
                    df.Create();
                }
                df = new DirectoryInfo(dir);
                if (!df.Exists)
                {
                    df.Create();
                }

                string message = string.Format(format, args);

                DateTime dt = DateTime.Now;
                message = dt.ToString("[yyyy-MM-dd HH:mm:ss] ") + message;

                string fileName = dt.ToString("yyyy-MM-dd") + ".log";

                logBuySell.Items.Add(message);
                logBuySell.SelectedIndex = logBuySell.Items.Count - 1;

                File.AppendAllText(dir + fileName, message + "\r\n");
            }));
        }


        #endregion [ETC]

        
    }
}

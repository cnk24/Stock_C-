using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KiwoomStock
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }


        #region [FORM EVENT]

        private void frmMain_Load(object sender, EventArgs e)
        {
            var communicator = new MsgBusCommunicator();
            var stub = new MsgBusStub(communicator);

            communicator.StartPumpMsgInBackground();
            stub.HandlePythonWorldInvokeForEver();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        #endregion [FORM EVENT]


    }
}

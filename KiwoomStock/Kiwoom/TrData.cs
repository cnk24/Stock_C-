using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiwoom
{
    public class TrData
    {
        /// <summary>
        /// 조회명
        /// </summary>
        public string RQName
        {
            get;
            set;
        }

        public int 예수금
        {
            get;
            set;
        }


        #region [계좌평가잔고내역요청 - opw00018]
        public List<string> 종목번호
        {
            get;
            set;
        }

        public List<string> 종목명
        {
            get;
            set;
        }

        /// <summary>
        /// 수익률(%)
        /// </summary>
        public List<string> 수익률
        {
            get;
            set;
        }

        public List<int> 매입가
        {
            get;
            set;
        }

        public List<int> 보유수량
        {
            get;
            set;
        }

        public List<int> 매매가능수량
        {
            get;
            set;
        }

        public List<int> 현재가
        {
            get;
            set;
        }

        public List<int> 매입금액
        {
            get;
            set;
        }

        public List<string> 수수료합
        {
            get;
            set;
        }

        #endregion [계좌평가잔고내역요청-opw00018]


    }
}

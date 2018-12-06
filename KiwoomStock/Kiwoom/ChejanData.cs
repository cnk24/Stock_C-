using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiwoom
{
    /// <summary>
    /// 체결 데이터
    /// </summary>
    public class ChejanData
    {
        /// <summary>
        /// 주문상태
        /// 접수 : 체결
        /// </summary>
        public string 주문상태
        {
            get;
            set;
        }

        /// <summary>
        /// 매매구분
        /// +매수 : -매도
        /// </summary>
        public string 매매구분
        {
            get;
            set;
        }

        /// <summary>
        /// 종목코드
        /// </summary>
        public string 종목코드
        {
            get;
            set;
        }

        /// <summary>
        /// 종목명
        /// </summary>
        public string 종목명
        {
            get;
            set;
        }

        /// <summary>
        /// 주문가
        /// </summary>
        public int 주문가
        {
            get;
            set;
        }

        /// <summary>
        /// 주문수량
        /// </summary>
        public int 주문수량
        {
            get;
            set;
        }

        /// <summary>
        /// 체결가
        /// </summary>
        public int 체결가
        {
            get;
            set;
        }

        /// <summary>
        /// 체결수량
        /// </summary>
        public int 체결수량
        {
            get;
            set;
        }
    }
}

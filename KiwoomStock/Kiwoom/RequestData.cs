using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiwoom
{
    /// <summary>
    /// 시세 데이터
    /// </summary>
    public class RequestData
    {
        /// <summary>
        /// 종목코드
        /// </summary>
        public string 종목코드
        {
            get;
            set;
        }

        /// <summary>
        /// 현재가
        /// </summary>
        public int 현재가
        {
            get;
            set;
        }

        /// <summary>
        /// 거래량
        /// </summary>
        public int 거래량
        {
            get;
            set;
        }
    }
}

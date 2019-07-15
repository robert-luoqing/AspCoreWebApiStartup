using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Model.Common
{
    public class WsListRequest<T>
    {
        public WsPager Pager { get; set; }
        public T Condition { get; set; }
    }
}

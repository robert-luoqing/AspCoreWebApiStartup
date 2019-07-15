using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Biz.Common
{
    /// <summary>
    /// It is all business base class
    /// </summary>
    public abstract class ManagerBase
    {
        public BizContext BizContext { get; }
        public ManagerBase(BizContext bizContext)
        {
            this.BizContext = bizContext;
        }
    }
}

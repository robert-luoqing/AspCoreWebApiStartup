using SuperPartner.Model.Common;
using SuperPartner.Model.Organization.User;
using SuperPartner.Permission.TokenHandler;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Biz.Common
{
    /// <summary>
    /// Author: Robert
    /// It is SuperPartner framework context.
    /// It will include token, language, user etc.
    /// It will use dependency inject to inject the class
    /// The type is "scoped" mean it will create per request in SpAuthFilter
    /// </summary>
    public class BizContext
    {
        public BizContext(SpConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// The app setting configuration
        /// </summary>
        public SpConfiguration Configuration { get; set; }
        /// <summary>
        /// Token handler
        /// </summary>
        public ITokenHandler TokenHandler { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Current login user
        /// </summary>
        public LoginUser LoginUser { get; set; }
    }
}

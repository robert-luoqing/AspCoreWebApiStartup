using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Permission.Authorization
{
    /// <summary>
    /// Authorization function assignation model
    /// </summary>
    public class FuncAssignation
    {
        /// <summary>
        /// Function code
        /// </summary>
        public string FuncCode { get; set; }

        /// <summary>
        /// The level if required
        /// For example: I can access difference type of Customer data
        /// The difference type means "My Customers", "My Department Customers", "My Organization Customers", "My Company Customers"
        /// Then, The FuncCode will be "Customer" or "CustomerList"
        /// The
        /// </summary>
        public AccessLevel? AccessLevel { get; set; }
    }
}

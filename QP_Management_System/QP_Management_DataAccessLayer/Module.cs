//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QP_Management_DataAccessLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class Module
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Module()
        {
            this.QPMasterPools = new HashSet<QPMasterPool>();
        }
    
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public Nullable<int> FAId { get; set; }
        public Nullable<float> Marks { get; set; }
        public System.DateTime CreationLog { get; set; }
        public Nullable<System.DateTime> UpdationLog { get; set; }
        public string Comments { get; set; }
    
        public virtual FocusArea FocusArea { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QPMasterPool> QPMasterPools { get; set; }
    }
}

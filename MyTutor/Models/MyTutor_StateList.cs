//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyTutor.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MyTutor_StateList
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MyTutor_StateList()
        {
            this.MyTutor_CityList = new HashSet<MyTutor_CityList>();
        }
    
        public int StateId { get; set; }
        public string States { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MyTutor_CityList> MyTutor_CityList { get; set; }
    }
}

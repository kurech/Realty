//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace realty
{
    using System;
    using System.Collections.Generic;
    
    public partial class Offer
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public int Id_agent { get; set; }
        public int Id_client { get; set; }
        public int Id_realestate { get; set; }
    
        public virtual Agent Agent { get; set; }
        public virtual Client Client { get; set; }
        public virtual RealEstate RealEstate { get; set; }
    }
}

﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebClient.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EntityEntities : DbContext
    {
        public EntityEntities()
            : base("name=EntityEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<QUESTION> QUESTIONs { get; set; }
        public virtual DbSet<QUESTION_GROUP> QUESTION_GROUP { get; set; }
        public virtual DbSet<QUESTION_OPTION> QUESTION_OPTION { get; set; }
        public virtual DbSet<QUESTION_TYPE> QUESTION_TYPE { get; set; }
        public virtual DbSet<SURVEY> SURVEYs { get; set; }
        public virtual DbSet<TYPE_SURVEY> TYPE_SURVEY { get; set; }
        public virtual DbSet<QUESTION_TICKET> QUESTION_TICKET { get; set; }
        public virtual DbSet<SURVEY_STATUS> SURVEY_STATUS { get; set; }
        public virtual DbSet<CLASS_ENTITY> CLASS_ENTITY { get; set; }
        public virtual DbSet<PERSON_ENTITY> PERSON_ENTITY { get; set; }
        public virtual DbSet<SURVEY_TICKET> SURVEY_TICKET { get; set; }
        public virtual DbSet<SURVEY_ACCOUNT_TYPE_TICKET> SURVEY_ACCOUNT_TYPE_TICKET { get; set; }
        public virtual DbSet<SURVEY_TICKET_ACTIVITY> SURVEY_TICKET_ACTIVITY { get; set; }
        public virtual DbSet<ACCOUNT_ENTITY> ACCOUNT_ENTITY { get; set; }
        public virtual DbSet<SPAM_CONTROL> SPAM_CONTROL { get; set; }
    }
}

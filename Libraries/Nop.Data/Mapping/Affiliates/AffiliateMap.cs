using Nop.Core.Domain.Affiliates;

namespace Nop.Data.Mapping.Affiliates
{
    public partial class AffiliateMap : NopEntityTypeConfiguration<Affiliate>
    {
        public AffiliateMap()
        {
            this.ToTable("Affiliate");
            this.HasKey(a => a.Id);
            this.Property(a => a.Level).IsRequired();

            this.HasRequired(a => a.Address).WithMany().HasForeignKey(x => x.AddressId).WillCascadeOnDelete(false);
            this.HasOptional(a => a.ParentAffiliate)
                .WithMany()
                .HasForeignKey(a => a.ParentAffiliateId).WillCascadeOnDelete(false);
        }
    }
}
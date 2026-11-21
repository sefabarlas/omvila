using NodaTime;
using Omvila.Modules.Scheduling.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Tests;

// Kural: alan-modeli.md § AppointmentItem ("Paketten düşülen kalemde PriceMinor yine yazılır, tahsil edilecek tutar 0 olur");
// durum-makinesi.md A1 (paket seansı ayrılır), A11 (tamamla: "Paketten"); sözleşme manageCompleteAppointment tutar kuralı
// (alınacak = kalemler − alınmış kapora − paketten düşülen kalemler).
public sealed class PackageItemTests
{
    private static readonly Instant Start = Offers.Tuesday1430;
    private static readonly Guid UserId = Guid.Parse("0193a8f0-0000-7000-8000-0000000000a1");
    private static readonly Guid PackageItem = Guid.Parse("0193a8f0-0000-7000-8000-0000000007a1");
    private static readonly Guid PaymentId = Guid.Parse("0193a8f0-0000-7000-8000-0000000007b1");
    private readonly SequentialIds _ids = new();

    [Fact]
    public void A1_PackageItem_PriceKept_ExcludedFromAmountDue()
    {
        var appointment = Create(AppointmentChannel.Online, packageItems: new Dictionary<int, Guid> { [2] = PackageItem });

        var blowDry = appointment.Items.Single(i => i.SeqNo == 2);
        blowDry.CustomerPackageItemId.ShouldBe(PackageItem);
        blowDry.IsFromPackage.ShouldBeTrue();
        blowDry.PriceMinor.ShouldBe(25_000, "fiyat kopyası yazılır");
        appointment.Items.Single(i => i.SeqNo == 1).IsFromPackage.ShouldBeFalse();
        appointment.UsesPackage.ShouldBeTrue();
        appointment.TotalAmountMinor.ShouldBe(70_000);
        appointment.PackageCoveredMinor.ShouldBe(25_000);
        appointment.PayableAmountMinor.ShouldBe(45_000);
        appointment.AmountDueMinor.ShouldBe(45_000, "Salonda ödenecek paketten düşüleni içermez");
    }

    [Fact]
    public void AmountDue_DepositNeverExceedsPayable()
    {
        var appointment = Create(
            AppointmentChannel.Online,
            new DepositHold(60_000, "OMV-7K2X", Duration.FromHours(2)),
            new Dictionary<int, Guid> { [2] = PackageItem });

        appointment.MarkDepositReceived(PaymentId, Start - Duration.FromDays(2) + Duration.FromHours(1), UserId).IsSuccess.ShouldBeTrue();

        appointment.RetainedDepositMinor.ShouldBe(45_000, "kapora tahsil edilecek tutarla sınırlı");
        appointment.AmountDueMinor.ShouldBe(0);
    }

    [Fact]
    public void A11_RedeemFromPackage_ThenCompleteWithReducedAmount()
    {
        var appointment = Create(AppointmentChannel.WalkIn);
        var blowDry = appointment.Items.Single(i => i.SeqNo == 2);

        appointment.RedeemFromPackage(blowDry.Id, PackageItem, "packageRedemptions[0].appointmentItemId").IsSuccess.ShouldBeTrue();

        blowDry.CustomerPackageItemId.ShouldBe(PackageItem);
        appointment.AmountDueMinor.ShouldBe(45_000);
        appointment.Complete(70_000, Start, UserId).Error!.Meta[Appointment.ExpectedAmountMeta].ShouldBe(45_000L);
        appointment.Complete(45_000, Start, UserId).IsSuccess.ShouldBeTrue();
        blowDry.Status.ShouldBe(AppointmentItemStatus.Completed);
    }

    [Fact]
    public void A11_RedeemFromPackage_UnknownOrAlreadyPackagedItem_IsValidationError()
    {
        var appointment = Create(AppointmentChannel.WalkIn, packageItems: new Dictionary<int, Guid> { [2] = PackageItem });
        var field = "packageRedemptions[0].appointmentItemId";

        appointment.RedeemFromPackage(PaymentId, PackageItem, field).Error!.FieldErrors[field].ShouldBe(["out_of_range"]);
        appointment.RedeemFromPackage(appointment.Items.Single(i => i.SeqNo == 2).Id, PackageItem, field).Error!.FieldErrors[field]
            .ShouldBe(["out_of_range"], "zaten paketten düşülüyor");
    }

    [Fact]
    public void A11_RedeemFromPackage_NotCheckedIn_IsInvalidStateTransition()
    {
        var appointment = Create(AppointmentChannel.Phone);

        appointment.RedeemFromPackage(appointment.Items[0].Id, PackageItem, "x").Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        appointment.Items.ShouldAllBe(i => !i.IsFromPackage);
    }

    [Fact]
    public void Cancelled_PackageItemNoLongerCovered()
    {
        var appointment = Create(AppointmentChannel.Phone, packageItems: new Dictionary<int, Guid> { [2] = PackageItem });

        appointment.CancelBySalon(CancellationReason.CustomerRequested, null, UserId, Start - Duration.FromDays(1)).IsSuccess.ShouldBeTrue();

        appointment.UsesPackage.ShouldBeTrue("iptalde seansın geri dönmesi için işaret kalır");
        appointment.PackageCoveredMinor.ShouldBe(0);
    }

    private Appointment Create(AppointmentChannel channel, DepositHold? deposit = null, IReadOnlyDictionary<int, Guid>? packageItems = null) =>
        Appointment.Create(
            new NewAppointment(
                Guid.Parse("0193a8f0-0000-7000-8000-0000000000b1"),
                Guid.Parse("0193a8f0-0000-7000-8000-0000000000b2"),
                Guid.Parse("0193a8f0-0000-7000-8000-0000000000c1"),
                Guid.Parse("0193a8f0-0000-7000-8000-0000000000c2"),
                channel,
                ClientApp.Panel,
                CustomerNote: null,
                StaffNote: null,
                SeriesId: null,
                CreatedByUserId: null,
                Deposit: deposit,
                PackageItemsBySeqNo: packageItems),
            AppointmentPlanner.Plan(Start, [Offers.Haircut(), Offers.BlowDry()]).Value,
            _ids,
            Start - Duration.FromDays(2));
}

//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/customer_deletion_preview_active_packages_inner.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'customer_deletion_preview.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CustomerDeletionPreview {
  /// Returns a new [CustomerDeletionPreview] instance.
  CustomerDeletionPreview({

    required  this.fullName,

    required  this.appointmentCount,

    required  this.paymentCount,

    required  this.upcomingAppointmentCount,

    required  this.activePackages,
  });

      /// Onay için yazılması gereken ad
  @JsonKey(
    
    name: r'fullName',
    required: true,
    includeIfNull: false,
  )


  final String fullName;



      /// \"Korunacak (anonim): 12 randevu\"
  @JsonKey(
    
    name: r'appointmentCount',
    required: true,
    includeIfNull: false,
  )


  final int appointmentCount;



      /// \"9 ödeme\"
  @JsonKey(
    
    name: r'paymentCount',
    required: true,
    includeIfNull: false,
  )


  final int paymentCount;



      /// İptal edilecek yaklaşan randevular (neden AccountDeleted)
  @JsonKey(
    
    name: r'upcomingAppointmentCount',
    required: true,
    includeIfNull: false,
  )


  final int upcomingAppointmentCount;



      /// \"Aktif paket\" uyarısı
  @JsonKey(
    
    name: r'activePackages',
    required: true,
    includeIfNull: false,
  )


  final List<CustomerDeletionPreviewActivePackagesInner> activePackages;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CustomerDeletionPreview &&
      other.fullName == fullName &&
      other.appointmentCount == appointmentCount &&
      other.paymentCount == paymentCount &&
      other.upcomingAppointmentCount == upcomingAppointmentCount &&
      other.activePackages == activePackages;

    @override
    int get hashCode =>
        fullName.hashCode +
        appointmentCount.hashCode +
        paymentCount.hashCode +
        upcomingAppointmentCount.hashCode +
        activePackages.hashCode;

  factory CustomerDeletionPreview.fromJson(Map<String, dynamic> json) => _$CustomerDeletionPreviewFromJson(json);

  Map<String, dynamic> toJson() => _$CustomerDeletionPreviewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}


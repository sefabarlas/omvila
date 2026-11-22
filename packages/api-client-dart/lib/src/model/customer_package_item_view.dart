//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'customer_package_item_view.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CustomerPackageItemView {
  /// Returns a new [CustomerPackageItemView] instance.
  CustomerPackageItemView({

    required  this.customerPackageItemId,

    required  this.serviceId,

    required  this.serviceName,

    required  this.totalSessions,

    required  this.remainingSessions,
  });

  @JsonKey(
    
    name: r'customerPackageItemId',
    required: true,
    includeIfNull: false,
  )


  final String customerPackageItemId;



  @JsonKey(
    
    name: r'serviceId',
    required: true,
    includeIfNull: false,
  )


  final String serviceId;



  @JsonKey(
    
    name: r'serviceName',
    required: true,
    includeIfNull: false,
  )


  final String serviceName;



  @JsonKey(
    
    name: r'totalSessions',
    required: true,
    includeIfNull: false,
  )


  final int totalSessions;



      /// Defterden hesaplanır (geri dönen kullanım sayılmaz)
  @JsonKey(
    
    name: r'remainingSessions',
    required: true,
    includeIfNull: false,
  )


  final int remainingSessions;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CustomerPackageItemView &&
      other.customerPackageItemId == customerPackageItemId &&
      other.serviceId == serviceId &&
      other.serviceName == serviceName &&
      other.totalSessions == totalSessions &&
      other.remainingSessions == remainingSessions;

    @override
    int get hashCode =>
        customerPackageItemId.hashCode +
        serviceId.hashCode +
        serviceName.hashCode +
        totalSessions.hashCode +
        remainingSessions.hashCode;

  factory CustomerPackageItemView.fromJson(Map<String, dynamic> json) => _$CustomerPackageItemViewFromJson(json);

  Map<String, dynamic> toJson() => _$CustomerPackageItemViewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}


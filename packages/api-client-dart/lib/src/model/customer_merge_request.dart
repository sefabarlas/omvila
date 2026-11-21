//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'customer_merge_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CustomerMergeRequest {
  /// Returns a new [CustomerMergeRequest] instance.
  CustomerMergeRequest({

    required  this.sourceBrandCustomerId,

     this.mergeNotes = true,
  });

      /// \"Birleşecek kayıt\"; arşive kalkar
  @JsonKey(
    
    name: r'sourceBrandCustomerId',
    required: true,
    includeIfNull: false,
  )


  final String sourceBrandCustomerId;



      /// \"Notları birleştir\": iki kaydın notları tarih sırasıyla eklenir
  @JsonKey(
    defaultValue: true,
    name: r'mergeNotes',
    required: false,
    includeIfNull: false,
  )


  final bool? mergeNotes;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CustomerMergeRequest &&
      other.sourceBrandCustomerId == sourceBrandCustomerId &&
      other.mergeNotes == mergeNotes;

    @override
    int get hashCode =>
        sourceBrandCustomerId.hashCode +
        mergeNotes.hashCode;

  factory CustomerMergeRequest.fromJson(Map<String, dynamic> json) => _$CustomerMergeRequestFromJson(json);

  Map<String, dynamic> toJson() => _$CustomerMergeRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}


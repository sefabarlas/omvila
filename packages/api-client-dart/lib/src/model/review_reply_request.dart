//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'review_reply_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ReviewReplyRequest {
  /// Returns a new [ReviewReplyRequest] instance.
  ReviewReplyRequest({

    required  this.text,
  });

      /// \"Yanıtınızı yazın, müşteri görür\"
  @JsonKey(
    
    name: r'text',
    required: true,
    includeIfNull: false,
  )


  final String text;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ReviewReplyRequest &&
      other.text == text;

    @override
    int get hashCode =>
        text.hashCode;

  factory ReviewReplyRequest.fromJson(Map<String, dynamic> json) => _$ReviewReplyRequestFromJson(json);

  Map<String, dynamic> toJson() => _$ReviewReplyRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}


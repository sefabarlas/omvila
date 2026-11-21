//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

import 'package:dio/dio.dart';
import 'package:omvila_api/src/auth/api_key_auth.dart';
import 'package:omvila_api/src/auth/basic_auth.dart';
import 'package:omvila_api/src/auth/bearer_auth.dart';
import 'package:omvila_api/src/auth/oauth.dart';
import 'package:omvila_api/src/api/manage_auth_api.dart';
import 'package:omvila_api/src/api/manage_branches_api.dart';
import 'package:omvila_api/src/api/manage_brand_api.dart';
import 'package:omvila_api/src/api/manage_calendar_api.dart';
import 'package:omvila_api/src/api/manage_catalog_api.dart';
import 'package:omvila_api/src/api/manage_customers_api.dart';
import 'package:omvila_api/src/api/manage_notifications_api.dart';
import 'package:omvila_api/src/api/manage_packages_api.dart';
import 'package:omvila_api/src/api/manage_reviews_api.dart';
import 'package:omvila_api/src/api/manage_sales_api.dart';
import 'package:omvila_api/src/api/manage_session_api.dart';
import 'package:omvila_api/src/api/manage_staff_api.dart';
import 'package:omvila_api/src/api/manage_waitlist_api.dart';
import 'package:omvila_api/src/api/manage_walk_ins_api.dart';
import 'package:omvila_api/src/api/public_appointment_link_api.dart';
import 'package:omvila_api/src/api/public_booking_api.dart';
import 'package:omvila_api/src/api/public_review_api.dart';
import 'package:omvila_api/src/api/public_waitlist_api.dart';
import 'package:omvila_api/src/api/webhooks_api.dart';

class OmvilaApi {
  static const String basePath = r'http://localhost:5080';

  final Dio dio;
  OmvilaApi({
    Dio? dio,
    String? basePathOverride,
    List<Interceptor>? interceptors,
  })  : 
        this.dio = dio ??
            Dio(BaseOptions(
              baseUrl: basePathOverride ?? basePath,
              connectTimeout: const Duration(milliseconds: 5000),
              receiveTimeout: const Duration(milliseconds: 3000),
            )) {
    if (interceptors == null) {
      this.dio.interceptors.addAll([
        OAuthInterceptor(),
        BasicAuthInterceptor(),
        BearerAuthInterceptor(),
        ApiKeyAuthInterceptor(),
      ]);
    } else {
      this.dio.interceptors.addAll(interceptors);
    }
  }

  void setOAuthToken(String name, String token) {
    if (this.dio.interceptors.any((i) => i is OAuthInterceptor)) {
      (this.dio.interceptors.firstWhere((i) => i is OAuthInterceptor) as OAuthInterceptor).tokens[name] = token;
    }
  }

  /// Removes the OAuth token associated with the given [name].
  ///
  /// If no [OAuthInterceptor] is registered or no token exists for the given
  /// [name], this method has no effect.
  void removeOAuthToken(String name) {
    if (this.dio.interceptors.any((i) => i is OAuthInterceptor)) {
      (this.dio.interceptors.firstWhere((i) => i is OAuthInterceptor) as OAuthInterceptor).tokens.remove(name);
    }
  }

  void setBearerAuth(String name, String token) {
    if (this.dio.interceptors.any((i) => i is BearerAuthInterceptor)) {
      (this.dio.interceptors.firstWhere((i) => i is BearerAuthInterceptor) as BearerAuthInterceptor).tokens[name] = token;
    }
  }

  /// Removes the bearer authentication token associated with the given [name].
  ///
  /// If no [BearerAuthInterceptor] is registered or no token exists for the
  /// given [name], this method has no effect.
  void removeBearerAuth(String name) {
    if (this.dio.interceptors.any((i) => i is BearerAuthInterceptor)) {
      (this.dio.interceptors.firstWhere((i) => i is BearerAuthInterceptor) as BearerAuthInterceptor).tokens.remove(name);
    }
  }

  void setBasicAuth(String name, String username, String password) {
    if (this.dio.interceptors.any((i) => i is BasicAuthInterceptor)) {
      (this.dio.interceptors.firstWhere((i) => i is BasicAuthInterceptor) as BasicAuthInterceptor).authInfo[name] = BasicAuthInfo(username, password);
    }
  }

  /// Removes the basic authentication credentials associated with the given [name].
  ///
  /// If no [BasicAuthInterceptor] is registered or no credentials exist for the
  /// given [name], this method has no effect.
  void removeBasicAuth(String name) {
    if (this.dio.interceptors.any((i) => i is BasicAuthInterceptor)) {
      (this.dio.interceptors.firstWhere((i) => i is BasicAuthInterceptor) as BasicAuthInterceptor).authInfo.remove(name);
    }
  }

  void setApiKey(String name, String apiKey) {
    if (this.dio.interceptors.any((i) => i is ApiKeyAuthInterceptor)) {
      (this.dio.interceptors.firstWhere((element) => element is ApiKeyAuthInterceptor) as ApiKeyAuthInterceptor).apiKeys[name] = apiKey;
    }
  }

  /// Removes the API key associated with the given [name].
  ///
  /// If no [ApiKeyAuthInterceptor] is registered or no API key exists for the
  /// given [name], this method has no effect.
  void removeApiKey(String name) {
    if (this.dio.interceptors.any((i) => i is ApiKeyAuthInterceptor)) {
      (this.dio.interceptors.firstWhere((element) => element is ApiKeyAuthInterceptor) as ApiKeyAuthInterceptor).apiKeys.remove(name);
    }
  }

  /// Get ManageAuthApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ManageAuthApi getManageAuthApi() {
    return ManageAuthApi(dio);
  }

  /// Get ManageBranchesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ManageBranchesApi getManageBranchesApi() {
    return ManageBranchesApi(dio);
  }

  /// Get ManageBrandApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ManageBrandApi getManageBrandApi() {
    return ManageBrandApi(dio);
  }

  /// Get ManageCalendarApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ManageCalendarApi getManageCalendarApi() {
    return ManageCalendarApi(dio);
  }

  /// Get ManageCatalogApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ManageCatalogApi getManageCatalogApi() {
    return ManageCatalogApi(dio);
  }

  /// Get ManageCustomersApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ManageCustomersApi getManageCustomersApi() {
    return ManageCustomersApi(dio);
  }

  /// Get ManageNotificationsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ManageNotificationsApi getManageNotificationsApi() {
    return ManageNotificationsApi(dio);
  }

  /// Get ManagePackagesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ManagePackagesApi getManagePackagesApi() {
    return ManagePackagesApi(dio);
  }

  /// Get ManageReviewsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ManageReviewsApi getManageReviewsApi() {
    return ManageReviewsApi(dio);
  }

  /// Get ManageSalesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ManageSalesApi getManageSalesApi() {
    return ManageSalesApi(dio);
  }

  /// Get ManageSessionApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ManageSessionApi getManageSessionApi() {
    return ManageSessionApi(dio);
  }

  /// Get ManageStaffApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ManageStaffApi getManageStaffApi() {
    return ManageStaffApi(dio);
  }

  /// Get ManageWaitlistApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ManageWaitlistApi getManageWaitlistApi() {
    return ManageWaitlistApi(dio);
  }

  /// Get ManageWalkInsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ManageWalkInsApi getManageWalkInsApi() {
    return ManageWalkInsApi(dio);
  }

  /// Get PublicAppointmentLinkApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  PublicAppointmentLinkApi getPublicAppointmentLinkApi() {
    return PublicAppointmentLinkApi(dio);
  }

  /// Get PublicBookingApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  PublicBookingApi getPublicBookingApi() {
    return PublicBookingApi(dio);
  }

  /// Get PublicReviewApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  PublicReviewApi getPublicReviewApi() {
    return PublicReviewApi(dio);
  }

  /// Get PublicWaitlistApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  PublicWaitlistApi getPublicWaitlistApi() {
    return PublicWaitlistApi(dio);
  }

  /// Get WebhooksApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  WebhooksApi getWebhooksApi() {
    return WebhooksApi(dio);
  }
}

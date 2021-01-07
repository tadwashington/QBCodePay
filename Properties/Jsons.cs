using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Json用参照ライブラリ
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;


namespace QBCodePay.Properties
{
    /// <summary>
    /// 各種取引JSON定義集
    /// </summary>
    class Jsons
    {
#region "ユーザー認証送信用JSONフォーマット定義"

        /// <summary>
        /// ユーザー認証送信用JSON
        /// </summary>
        [JsonObject("userAuth")]
        public class UserAuth
        {
            [JsonProperty("loginId")]
            public string LoginId;
            [JsonProperty("userPassword")]
            public string UserPassword;
            [JsonProperty("osName")]
            public string OsName;
            [JsonProperty("osVersion")]
            public string OsVersion;
            [JsonProperty("serialNo")]
            public string SerialNo;

        }
        /// <summary>
        /// ユーザー認証受信用JSON(第1階層)
        /// </summary>
        [JsonObject("userAuthR")]
        public class UserAuthR
        {
            /// <summary>
            /// 結果コード
            /// </summary>
            [JsonProperty("returnCode")]
            public string ReturnCode;
            /// <summary>
            /// 結果メッセージ
            /// </summary>
            [JsonProperty("returnMessage")]
            public string ReturnMessage;
            /// <summary>
            /// 集約結果コード
            /// </summary>
            [JsonProperty("msgSummaryCode")]
            public string MsgSummaryCode;
            /// <summary>
            /// 集約結果メッセージ
            /// </summary>
            [JsonProperty("msgSummary")]
            public string MsgSummary;
            /// <summary>
            /// 詳細結果(第2階層)
            /// </summary>
            [JsonProperty("result")]
            public UserAuthResult Result;
            /// <summary>
            /// 利用後残高
            /// </summary>
            [JsonProperty("balanceAmount")]
            public string BalanceAmount;
        }
        /// <summary>
        /// ユーザー認証 結果詳細(第2階層)
        /// </summary>
        [JsonObject("userAuthResult")]
        public class UserAuthResult
        {
            /// <summary>
            /// 認証キー
            /// </summary>
            [JsonProperty("credentialKey")]
            public string CredentialKey;
            /// <summary>
            /// 企業名称
            /// </summary>
            [JsonProperty("partnerFullName")]
            public string PartnerFullName;
            /// <summary>
            /// 取引備考初期値
            /// </summary>
            [JsonProperty("description")]
            public string Description;
            /// <summary>
            /// 返金用認証パスワード
            /// </summary>
            [JsonProperty("adminPassword")]
            public string AdminPassword;
            /// <summary>
            /// 返金時認証要否フラグ
            /// </summary>
            [JsonProperty("authForRefund")]
            public string AuthForRefund;
            /// <summary>
            /// レジ番号
            /// </summary>
            [JsonProperty("cashNumber")]
            public string CashNumber;
            /// <summary>
            /// 警告文言
            /// </summary>
            [JsonProperty("warningWord")]
            public string WarningWord;
            /// <summary>
            /// 端末識別番号チェック要否
            /// </summary>
            [JsonProperty("checkSnFlag")]
            public string CheckSnFlag;
            /// <summary>
            /// 店舗名称
            /// </summary>
            [JsonProperty("merchantFullName")]
            public string MerchantFullName;
            /// <summary>
            /// 店舗略称
            /// </summary>
            [JsonProperty("merchantName")]
            public string MerchantName;
            /// <summary>
            /// 店舗名称 カナ
            /// </summary>
            [JsonProperty("merchantKanaName")]
            public string MerchantKanaName;
            /// <summary>
            /// 店舗住所の都道府県
            /// </summary>
            [JsonProperty("prefectures")]
            public string Prefectures;
            /// <summary>
            /// 店舗住所の市区町村
            /// </summary>
            [JsonProperty("city")]
            public string City;
            /// <summary>
            /// 店舗住所の番地
            /// </summary>
            [JsonProperty("street")]
            public string Street;
            /// <summary>
            /// 店舗住所
            /// </summary>
            [JsonProperty("address")]
            public string Address;
            /// <summary>
            /// 店舗電話番号
            /// </summary>
            [JsonProperty("contactPhoneNum")]
            public string ContactPhoneNum;
            /// <summary>
            /// 店舗メールアドレス
            /// </summary>
            [JsonProperty("email")]
            public string Email;
            /// <summary>
            /// 店舗ホームページ URL
            /// </summary>
            [JsonProperty("contactHomeUrl")]
            public string ContactHomeUrl;
            /// <summary>
            /// au PAYのQR提供方式
            /// </summary>
            [JsonProperty("qrProvisionMethod")]
            public string QrProvisionMethod;
            /// <summary>
            /// 店舗ID
            /// </summary>
            [JsonProperty("merchantId")]
            public string MerchantId;
            /// <summary>
            /// ﾕｰｻﾞ初期PW 変更済みﾌﾗｸﾞ
            /// 1：パスワード変更済み 0：初期パスワード未変更
            /// </summary>
            [JsonProperty("pwChangedFlag")]
            public string PwChangedFlag;
            /// <summary>
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            /// </summary>
            [JsonProperty("userId")]            // 削除予定
            public string UserId;
            /// <summary>
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            /// </summary>
            [JsonProperty("merchantCode")]      // 削除予定
            public string MerchantCode;
            /// <summary>
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            /// </summary>
            [JsonProperty("serialNo")]          // 削除予定
            public string SerialNo;
            /// <summary>
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            /// </summary>
            [JsonProperty("deviceId")]          // 削除予定
            public string DeviceId;
            /// <summary>
            /// 利用可能な決済種別リスト
            /// </summary>
            [JsonProperty("payTypeList")]
            public PayList PayTypeList;
        }
        /// <summary>
        /// 決済種別リスト （第三階層）
        /// </summary>
        [JsonObject("payList")]
        public class PayList
        {
            /// <summary>
            /// 決済種別ID
            /// </summary>
            [JsonProperty("payTypeId")]
            public string PayTypeId;
            /// <summary>
            /// 決済種別コード
            /// </summary>
            [JsonProperty("payTypeCode")]
            public string PayTypeCode;
            /// <summary>
            /// 決済種別名称
            /// </summary>
            [JsonProperty("payTypeName")]
            public string PayTypeName;
            /// <summary>
            /// 決済QRコード正規表現式
            /// </summary>
            [JsonProperty("qrcodeRegExr")]
            public string QrcodeRegExr;
            /// <summary>
            /// MPMフラグ
            /// </summary>
            [JsonProperty("dispQrcodeFlag")]
            public string DispQrcodeFlag;
            /// <summary>
            /// CPMフラグ
            /// </summary>
            [JsonProperty("readQrcodeFlag")]
            public string ReadQrcodeFlag;
        }
        #endregion
        #region "QRコード支払（CPM) API"
        /// <summary>
        /// QRコード支払（CPM) API送信用(PUT METHOD)
        /// </summary>
        [JsonObject("cpmReq")]
        public class CpmReq
        {
            /// <summary>
            /// 支払伝票番号
            /// </summary>
            [JsonProperty("order_id")]
            public string Order_id;
            /// <summary>
            /// 端末識別番号
            /// </summary>
            [JsonProperty("serialNo")]
            public string SerialNo;
            /// <summary>
            /// 取引備考
            /// </summary>
            [JsonProperty("description")]
            public string Description;
            /// <summary>
            /// 支払金額
            /// </summary>
            [JsonProperty("price")]
            public string Price;
            /// <summary>
            /// 決済QRコード
            /// </summary>
            [JsonProperty("auth_code")]
            public string Auth_code;
            /// <summary>
            /// 通貨
            /// </summary>
            [JsonProperty("currency")]
            public string Currency;
            /// <summary>
            /// ログインID
            /// </summary>
            [JsonProperty("operator")]
            public string Operator;
        }
        /// <summary>
        /// QRコード支払（CPM) API受信<リターン>用(PUT METHOD)
        /// 第1階層
        /// </summary>
        [JsonObject("cpmRes")]
        public class CpmRes
        {
            /// <summary>
            /// 結果コード
            /// </summary>
            [JsonProperty("returnCode")]
            public string ReturnCode;
            /// <summary>
            /// 結果メッセージ
            /// </summary>
            [JsonProperty("returnMessage")]
            public string ReturnMessage;
            /// <summary>
            /// 集約結果コード
            /// </summary>
            [JsonProperty("msgSummaryCode")]
            public string MsgSummaryCode;
            /// <summary>
            /// 集約結果メッセージ
            /// </summary>
            [JsonProperty("msgSummary")]
            public string MsgSummary;
            /// <summary>
            /// 結果詳細(第2階層へ)
            /// </summary>
            [JsonProperty("result")]
            public CpmResult Result;
            /// <summary>
            /// 利用後残高
            /// </summary>
            [JsonProperty("balanceAmount")]
            public string BalanceAmount;
        }
        /// <summary>
        /// 詳細結果(第2階層)
        /// </summary>
        [JsonObject("cpmResult")]
        public class CpmResult
        {
            /// <summary>
            /// 支払伝票番号
            /// </summary>
            [JsonProperty("partner_order_id")]
            public string Partner_order_id;
            /// <summary>
            /// 通貨
            /// </summary>
            [JsonProperty("currency")]
            public string Currency;
            /// <summary>
            /// 決済会社の決済ID
            /// </summary>
            [JsonProperty("order_id")]
            public string Order_id;
            /// <summary>
            /// 実行結果コード
            /// </summary>
            [JsonProperty("return_code")]
            public string Return_code;
            /// <summary>
            /// 処理結果コード
            /// </summary>
            [JsonProperty("result_code")]
            public string Result_code;
            /// <summary>
            /// レコード作成日時
            /// </summary>
            [JsonProperty("Create_time")]
            public string Create_time;
            /// <summary>
            /// 要求決済金額
            /// </summary>
            [JsonProperty("total_fee")]
            public string Total_fee;
            /// <summary>
            /// 決済金額
            /// </summary>
            [JsonProperty("real_fee")]
            public string Real_fee;
            /// <summary>
            /// 決済種別
            /// </summary>
            [JsonProperty("channel")]
            public string Channel;
            /// <summary>
            /// 支払日時
            /// </summary>
            [JsonProperty("pay_time")]
            public string Pay_time;
            /// <summary>
            /// 取引備考
            /// </summary>
            [JsonProperty("order_body")]
            public string Order_body;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("return_msg")]
            public string Return_msg;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("codeUrl")]
            public string CodeUrl;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("record_id")]
            public string Record_id;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("record_type")]
            public string Record_type;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("transaction_time")]
            public string Transaction_time;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("type")]
            public string Type;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("pre_authorization")]
            public string Pre_authorization;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("total_amount")]
            public string Total_amount;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("input_amount")]
            public string Input_amount;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("customer_payment_amount")]
            public string Customer_payment_amount;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("exchange_rate")]
            public string Exchange_rate;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("remark")]
            public string Remark;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("Settle_amount")]
            public string Settle_amount;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("Refund_fee")]
            public string Refund_fee;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("sum_refund_fee")]
            public string sum_refund_fee;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("amount")]
            public string amount;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("tip_amount")]
            public string Tip_amount;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("partner_code")]
            public string Partner_code;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("refund_id")]
            public string Refund_id;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("partner_refund_id")]
            public string Partner_refund_id;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("clearing_status")]
            public string Clearing_status;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("gateway")]
            public string Gateway;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("status")]
            public string Status;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("agencyFullName")]
            public string AgencyFullName;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("partnerFullName")]
            public string PartnerFullName;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("partner_name")]
            public string Partner_name;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("merchantId")]
            public string MerchantId;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("merchantCode")]
            public string MerchantCode;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("merchantFullName")]
            public string MerchantFullName;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("merchantName")]
            public string MerchantName;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("orderId")]
            public string OrderId;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("merchantOrderId")]
            public string MerchantOrderId;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("rate")]
            public string Rate;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("res_result_code")]
            public string Res_result_code;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("ipAddress")]
            public string IpAddress;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("billingNumber")]
            public string BillingNumber;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("payAmount")]
            public string PayAmount;
        }
        /// <summary>
        /// 支払結果確認 API
        /// 「QRコード支払（ CPM ）」 APIにて要求した決済処理の結果照会を行う API
        /// </summary>
        [JsonObject("cpmCheck")]
        public class CpmCheck
        {
            /// <summary>
            /// 結果コード
            /// </summary>
            [JsonProperty("returnCode")]
            public string ReturnCode;
            /// <summary>
            /// 結果メッセージ
            /// </summary>
            [JsonProperty("returnMessage")]
            public string ReturnMessage;
            /// <summary>
            /// 集約結果コード
            /// </summary>
            [JsonProperty("msgSummaryCode")]
            public string MsgSummaryCode;
            /// <summary>
            /// 集約結果メッセージ
            /// </summary>
            [JsonProperty("msgSummary")]
            public string MsgSummary;
            /// <summary>
            /// 結果詳細（第二階層へ）
            /// </summary>
            [JsonProperty("result")]
            public ChkResult Result;
            /// <summary>
            /// 利用後残高
            /// </summary>
            [JsonProperty("balanceAmount")]
            public string BalanceAmount;
        }
        /// <summary>
        /// 結果詳細（第二階層）
        /// </summary>
        [JsonObject("chkResult")]
        public class ChkResult
        {
            /// <summary>
            /// 支払伝票番号
            /// </summary>
            [JsonProperty("partner_order_id")]
            public string Partner_order_id;
            /// <summary>
            /// 通貨
            /// </summary>
            [JsonProperty("currency")]
            public string Currency;
            /// <summary>
            /// 決済会社の決済ID
            /// </summary>
            [JsonProperty("order_id")]
            public string Order_id;
            /// <summary>
            /// 実行結果コード
            /// </summary>
            [JsonProperty("return_code")]
            public string Return_code;
            /// <summary>
            /// 処理結果コード
            /// </summary>
            [JsonProperty("result_code")]
            public string Result_code;
            /// <summary>
            /// 支払金額
            /// </summary>
            [JsonProperty("real_fee")]
            public string Real_fee;
            /// <summary>
            /// 決済種別
            /// </summary>
            [JsonProperty("channel")]
            public string Channel;
            /// <summary>
            /// レコード作成日時
            /// </summary>
            [JsonProperty("create_time")]
            public string Create_time;
            /// <summary>
            /// 要求決済金額
            /// </summary>
            [JsonProperty("total_fee")]
            public string Total_fee;
            /// <summary>
            /// 支払日時
            /// </summary>
            [JsonProperty("pay_time")]
            public string Pay_time;
            /// <summary>
            /// 返金金額
            /// </summary>
            [JsonProperty("refund_fee")]
            public string Refund_fee;
            /// <summary>
            /// 取引備考
            /// </summary>
            [JsonProperty("order_body")]
            public string Order_body;
            /// <summary>
            /// 決済ステータス
            /// </summary>
            [JsonProperty("status")]
            public string Status;
            /// <summary>
            /// 一部返金可否フラグ
            /// </summary>
            [JsonProperty("partialRefundFlag")]
            public string PartialRefundFlag;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("return_msg")]
            public string Return_msg;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("codeUrl")]
            public string CodeUrl;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("record_id")]
            public string Record_id;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("record_type")]
            public string Record_type;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("transaction_time")]
            public string Transaction_time;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("type")]
            public string Type;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("pre_authorization")]
            public string Pre_authorization;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("total_amount")]
            public string Total_amount;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("input_amount")]
            public string Input_amount;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("customer_payment_amount")]
            public string Customer_payment_amount;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("exchange_rate")]
            public string Exchange_rate;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("remark")]
            public string Remark;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("settle_amount")]
            public string Settle_amount;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("sum_refund_fee")]
            public string Sum_refund_fee;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("amount")]
            public string Amount;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("tip_amount")]
            public string Tip_amount;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("partner_code")]
            public string Partner_code;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("refund_id")]
            public string Refund_id;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("partner_refund_id")]
            public string partner_refund_id;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("clearing_status")]
            public string Clearing_status;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("gateway")]
            public string Gateway;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("agencyFullName")]
            public string AgencyFullName;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("partnerFullName")]
            public string PartnerFullName;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("partner_name")]
            public string Partner_name;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("merchantId")]
            public string MerchantId;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("merchantCode")]
            public string MerchantCode;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("merchantFullName")]
            public string MerchantFullName;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("merchantName")]
            public string MerchantNamee;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("orderId")]
            public string OrderId;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("merchantOrderId")]
            public string MerchantOrderId;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("rate")]
            public string Rate;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("res_result_code")]
            public string Res_result_code;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("ipAddress")]
            public string IpAddress;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("billingNumber")]
            public string BillingNumber;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("payAmount")]
            public string payAmount;
        }

        #endregion
        #region "返金API"
        /// <summary>
        /// 返金API リクエスト送信用(PUT METHOD)
        /// </summary>
        [JsonObject("repayReq")]
        public class RepayReq
        {
            /// <summary>
            /// 返金伝票番号(店舗単位で一意の番号)
            /// 半角数字(20桁固定)-必須
            /// </summary>
            [JsonProperty("refund_id")]
            public string Refund_id;
            /// <summary>
            /// 対象支払伝票番号
            /// (返金対象の支払伝票番号「QRコード支払（ CPM ）」API 実行時に発番した支払伝票番号)
            /// 半角数字(20桁固定)-必須
            /// </summary>
            [JsonProperty("order_id")]
            public string Order_id;
            /// <summary>
            /// 端末識別番号
            /// 管理画面が発行した端末ユーザID（加盟店管理画面にて作成)
            /// 半角英数(128桁可変)
            /// </summary>
            [JsonProperty("serialNo")]
            public string SerialNo;
            /// <summary>
            /// 返金金額
            /// 返金できない金額が設定された場合、レスポンスがエラーとなる 。
            /// 半角数字(8桁可変)-必須
            /// </summary>
            [JsonProperty("fee")]
            public int Fee;
        }
        [JsonObject("repayRes")]
        public class RepayRes
        {
            /// <summary>
            /// 結果コード
            /// 半角英数記号(30桁可変-必須)
            /// </summary>
            [JsonProperty("returnCode")]
            public string ReturnCode;
            /// <summary>
            /// 結果メッセージ
            /// 全半角文字(200Byte可変)
            /// </summary>
            [JsonProperty("returnMessage")]
            public string ReturnMessage;
            /// <summary>
            /// 集約結果コード
            /// 半角英数(5桁-必須)
            /// </summary>
            [JsonProperty("msgSummaryCode")]
            public string MsgSummaryCode;
            /// <summary>
            /// 結果詳細(第2階層へ)
            /// </summary>
            [JsonProperty("result")]
            public RepayResult Result;
            /// <summary>
            /// 利用後残高
            /// 半角数字(10桁-可変)
            /// </summary>
            [JsonProperty("balanceAmount")]
            public int BalanceAmount;
        }
        /// <summary>
        /// 返金API 結果詳細(第2階層)
        /// </summary>
        [JsonObject("repayResult")]
        public class RepayResult
        {
            /// <summary>
            /// 返金伝票番号
            /// 半角数字(20桁固定)-リクエスト返金伝票番号と同じ
            /// </summary>
            [JsonProperty("partner_refund_id")]
            public string Partner_refund_id;
            /// <summary>
            /// 決済会社の返金ID
            /// 半角英数(128桁可変)
            /// </summary>
            [JsonProperty("refund_id")]
            public string Refund_id;
            /// <summary>
            /// 通貨
            /// 半角英大文字(3桁固定)
            /// </summary>
            [JsonProperty("currency")]
            public string Currency;
            /// <summary>
            /// 実行結果コード
            /// 半角英字(32桁可変)
            /// </summary>
            [JsonProperty("return_code")]
            public string Return_code;
            /// <summary>
            /// 処理結果コード
            /// 半角英字記号(32桁可変)
            /// </summary>
            [JsonProperty("result_code")]
            public string Result_code;
            /// <summary>
            /// 対象支払伝票番号
            /// 半角数字(20桁固定)
            /// </summary>
            [JsonProperty("partner_order_id")]
            public string Partner_order_id;
            /// <summary>
            /// 返金金額
            /// 半角数字(8桁可変)
            /// </summary>
            [JsonProperty("amount")]
            public int Amount;
            /// <summary>
            /// 決済種別
            /// 半角英字(8桁可変)
            /// </summary>
            [JsonProperty("channel")]
            public string Channel;
            /// <summary>
            /// 決済会社の支払決済ID
            /// 半角英数字(128桁可変)
            /// </summary>
            [JsonProperty("order_id")]
            public string Order_id;
            /// <summary>
            /// レコード作成日時
            /// 半角数字記号(19桁固定)-YYYY/MM/DD HH:MM:SS
            /// </summary>
            [JsonProperty("create_time")]
            public string Create_time;
            /// <summary>
            /// 返金日時
            /// 半角数字記号(19桁固定)-YYYY/MM/DD HH:MM:SS
            /// result_code(処理結果コード)="WAITING"(支払待ち)の場合は設定なし
            /// </summary>
            [JsonProperty("pay_time")]
            public string Pay_time;
            /// <summary>
            /// 要求決済金額
            /// 半角数字(8桁可変)-元支払伝票の決済リクエスト時の決済金額
            /// </summary>
            [JsonProperty("total_fee")]
            public int Total_fee;
            /// <summary>
            /// 決済金額
            /// 半角数字(8桁可変)-要求決済金額と同じ値
            /// </summary>
            [JsonProperty("real_fee")]
            public int Real_fee;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("sum_refund_fee")]
            public string Sum_refund_fee;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("return_msg")]
            public string Return_msg;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("codeUrl")]
            public string CodeUrl;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("record_id")]
            public string Record_id;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("record_type")]
            public string Record_type;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("transaction_time")]
            public string Transaction_time;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("type")]
            public string Type;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("pre_authorization")]
            public string Pre_authorization;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("total_amount")]
            public string Total_amount;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("input_amount")]
            public string Input_amount;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("customer_payment_amount")]
            public string Customer_payment_amount;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("exchange_rate")]
            public string Exchange_rate;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("remark")]
            public string Remark;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("settle_amount")]
            public string Settle_amount;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("refund_fee")]
            public string Refund_fee;
            /// 削除予定フィールド
            /// 当該フィールドが存在しない場合でもエラーを起こさないよう注意
            [JsonProperty("tip_amount")]
            public string Tip_amount;
        }
        #endregion
    }
}

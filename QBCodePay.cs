using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Text.RegularExpressions;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;

// Json用参照ライブラリ
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;



namespace QBCodePay
{
    public partial class QBCodePay : Form
    {
        #region "コンスタント群"
        /// <summary>
        /// Result_Code:PAY_SUCCESS(支払成功)
        /// </summary>
        private string cResult_Code_S = "PAY_SUCCESS";
        /// <summary>
        /// Result_Code:PAYING(支払待ち)
        /// </summary>
        private string cResult_Code_W = "PAYING";
        /// <summary>
        /// 結果コード(正常:MP10000)
        /// 以外は異常と判定する
        /// </summary>
        private string cReturnCode = "MP10000";
        #endregion

        #region "プロパティ群"
        /// <summary>
        /// API URL
        /// </summary>
        public string pUrl { get; set; }
        /// <summary>
        /// GET METHOD Pauling間隔(ミリ秒)
        /// </summary>
        public int pPollTime { get; set; }
        /// <summary>
        /// QRコード支払いタイムアウト(MAX30秒)
        /// </summary>
        public int pCPMTimeOut { get; set; }
        /// <summary>
        /// 返金タイムアウト(MAX300秒)
        /// </summary>
        public int pRefundTimeOut { get; set; }
        #endregion
        #region "インスタンス群"
        /// <summary>
        /// QRコード支払レスポンスJSONクラスインスタンス
        /// </summary>
        MakeJsons.CpmRes cpm;
        /// <summary>
        /// 支払い確認APIレスポンスJSONクラスインスタンス
        /// </summary>
        MakeJsons.CpmCheck resp;
        #endregion
        public QBCodePay()
        {
            InitializeComponent();
            // GET METHOD Pauling間隔を設定(2秒以上)
            pPollTime = 2000;
            // QRコード支払いTimeOut設定(30秒)
            pCPMTimeOut = 30000;
            // 返金TimeOut設定(300秒)
            pRefundTimeOut = 300000;
        }
        /// <summary>
        /// EndPoint入力チェック
        /// </summary>
        /// <returns></returns>
        private bool ChkEndPoint(string surl)
        {
            bool rtn = true;

            try
            {
                if (!string.IsNullOrEmpty(surl))
                {
                    rtn = Regex.IsMatch(surl,
                       @"^s?https?://[-_.!~*'()a-zA-Z0-9;/?:@&=+$,%#]+$"
                    );

                    if (rtn == false)
                    {
                        Console.WriteLine(string.Format("EndPointとして認識できない文字列です。"));
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("Urlを入力して下さい。"));
                    rtn = false;
                }
            }
            catch (Exception e)
            {
                rtn = false;
                Console.WriteLine(string.Format("EndPointとして認識できない文字列です。:{0}", e.Message));
            }
            finally
            {
                if (rtn == false)
                {
                    MessageBox.Show("EndPointとして認識できない文字列が入力されています。", "間違いなく間違ってます");
                }
            }
            return rtn;
        }
        /// <summary>
        /// GETボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGets_Click(object sender, EventArgs e)
        {
            pUrl = this.txtEndPint.Text;
            // GET METHOD実行
            Get_Method();
            this.txtEndPint.SelectAll();
            this.txtEndPint.Focus();
        }
        /// <summary>
        /// GET METHOD
        /// </summary>
        private async void Get_Method()
        {
            if (!ChkEndPoint(pUrl))
            {
                return;
            }
            bool rtn = await GetApiFrmUrl(pUrl);
            /* 
             * APIレスポンスのReturn_Codeが"MP10000(処理正常)"でかつメソッドリターンがfalseの場合は 
             * 支払待ちと判定し支払確認(GET METHOD)のPaulingを行う
             */
            if ((resp.ReturnCode == cReturnCode) && (rtn = false))
            {
                while (true)
                {
                    // GET METHOD PAULING
                    if (await GetPauling() == true)
                    {
                        // 処理正常でかつ支払完了ならPAULING終了
                        if ((resp.ReturnCode == cReturnCode) && (resp.Result.Result_code == cResult_Code_S))
                        {
                            // 支払完了
                            break;
                        }
                    }
                    else
                    {
                        if ((resp.ReturnCode != cReturnCode))
                        {
                            // エラー処理へ
                            break;
                        }

                    }
                }
            }

        }
        /// <summary>
        /// PUTボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPuts_Click(object sender, EventArgs e)
        {
            pUrl = this.txtEndPint.Text;
            // PUT METHOD実行
            Put_Method();

            this.txtEndPint.SelectAll();
            this.txtEndPint.Focus();

        }
        /// <summary>
        /// PUT METHOD 
        /// </summary>
        private async void Put_Method()
        {
            if (!ChkEndPoint(pUrl))
            {
                return;
            }

            string jdata = string.Empty;
            // QRコード支払(CPM)API[PUT METHOD]用 jsonデータ生成～httpRequest送信
            if (PutCPM(ref jdata))
            {
                // Put Method Request送信(jdata:JSONフォーマット)
                bool rtn = await PutApiFrmUrl(pUrl, jdata);
                // メソッドリターンがfalseでAPI処理が正常の場合は支払確認処理をPAULING
                if  ((!string.IsNullOrEmpty(cpm.ReturnCode)) && (!string.IsNullOrEmpty(cpm.Result.Result_code)))
                {
                    if ((rtn == false) && (cpm.ReturnCode == cReturnCode) && (cpm.Result.Result_code == cResult_Code_W))
                    {
                        while (true)
                        {
                            // GET METHOD PAULING
                            if (await GetPauling() == true)
                            {
                                // 処理正常でかつ支払完了ならPAULING終了
                                if ((!string.IsNullOrEmpty(resp.ReturnCode)) && (!string.IsNullOrEmpty(resp.Result.Result_code)) && 
                                    (resp.ReturnCode == cReturnCode) && (resp.Result.Result_code == cResult_Code_S))
                                {
                                    // 支払完了
                                    break;
                                }
                            }
                            else
                            {
                                if ((!string.IsNullOrEmpty(resp.ReturnCode)) && (resp.ReturnCode != cReturnCode))
                                {
                                    // エラー処理へ
                                    break;
                                }

                            }

                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("PUT METHOD RequestJSONファイルの生成に失敗しました。", "JSONファイル生成エラー");
            }

        }
        /// <summary>
        /// Get Method
        /// </summary>
        /// <param name="s"></param>
        private async Task<bool> GetApiFrmUrl(string s,int refund = 0)
        {
            bool rtn = false;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            try
            {
                // クエリパラメタ追記
                /*
                 * GETデータ（クエリパラメータ）
                 * storeOrderId(支払伝票番号)固定20桁を設定する
                 */
                // urlの末尾に「/」がない場合は追記する
                if (s.Substring(s.Length - 1) != "/")
                {
                    s += "/";
                }
                // 支払伝票番号(半角数字固定20桁)
                ulong n;
                if (refund == 0)
                {
                    // 支払伝票番号をセット
                    n = 11234567890123456789;
                    // クエリパラメタをセット
                    s += "?storeOrderId=" + n.ToString();
                }
                // HttpHeader編集
                var client = new HttpClient();
                // QRコード支払と返金処理のパラメタ設定
                if (refund == 0)
                {
                    // タイムアウト設定
                    client.Timeout = TimeSpan.FromMilliseconds(pCPMTimeOut);
                }
                else
                {
                    // タイムアウト設定
                    client.Timeout = TimeSpan.FromMilliseconds(pRefundTimeOut);
                }
                AddHttpHeader2(ref client);
                var res = await client.GetAsync(s);

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    // GetResponse取得成功
                    Console.WriteLine("GET成功！");
                    var g = await res.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(g))
                    {
                        resp = new MakeJsons.CpmCheck();
                        resp = JsonConvert.DeserializeObject<MakeJsons.CpmCheck>(g);
                        // 確認のためアイアログ表示
                        string rs =
                            string.Format("ReturnCode:{0} \r\n", resp.ReturnCode) + 
                            string.Format("ReturnMessage:{0}\r\n", resp.ReturnMessage) +
                            string.Format("MsgSummaryCode:{0}\r\n", resp.MsgSummaryCode) +
                            string.Format("MsgSummary:{0}\r\n", resp.MsgSummary) +
                            string.Format("Result.partner_refund_id:{0}\r\n", resp.Result.partner_refund_id) +
                            string.Format("Result.Currency:{0}\r\n", resp.Result.Currency) +
                            string.Format("Result.Order_id:{0}\r\n", resp.Result.Order_id) +
                            string.Format("Result.Return_code:{0}\r\n", resp.Result.Return_code) +
                            string.Format("Result.Result_code:{0}\r\n", resp.Result.Result_code) +
                            string.Format("Result.Real_fee:{0}\r\n", resp.Result.Real_fee) +
                            string.Format("Result.Channel:{0}\r\n", resp.Result.Channel) +
                            string.Format("Result.Create_time:{0}\r\n", resp.Result.Create_time) +
                            string.Format("Result.Total_fee:{0}\r\n", resp.Result.Total_fee) +
                            string.Format("Result.Pay_time:{0}\r\n", resp.Result.Pay_time) +
                            string.Format("Result.Refund_fee:{0}\r\n", resp.Result.Refund_fee) +
                            string.Format("Result.Order_body:{0}\r\n", resp.Result.Order_body) +
                            string.Format("Result.Status:{0}\r\n", resp.Result.Status) +
                            string.Format("Result.PartialRefundFlag:{0}\r\n", resp.Result.PartialRefundFlag) +
                            string.Format("BalanceAmount:{0}\r\n", resp.BalanceAmount)
                            ;
                        //MessageBox.Show(rs, "帰ってきたパラメタ");
                        Console.WriteLine(rs);
                        // API正常
                        if (resp.ReturnCode == cReturnCode)
                        {
                            // 支払完了
                            if(resp.Result.Result_code == cResult_Code_S)
                            {
                                rtn = true;
                                MessageBox.Show("支払確認が完了しました。", "支払確認");
                            }
                        }
                    }
                }
                else
                {
                    // GetResponse取得失敗
                    Console.WriteLine("GET失敗！");
                    var g = res.StatusCode.ToString();
                    MessageBox.Show(g, "GET失敗のStatusCode");
                }

                // http client開放
                client.Dispose();

            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("あかんやん！: {0}", e.Message));
                Console.WriteLine(string.Format("なんでやねん！: {0}", e.InnerException));
                MessageBox.Show(string.Format("Exceptionが発生しましたよ。:\r\n{0} \r\ninner Ex:\r\n{1}", e.Message, e.InnerException),
                    "残念ながらGETに失敗してます。");
            }

            return rtn;
        }
        /// <summary>
        /// PUT METHOD
        /// </summary>
        /// <param name="s">URL</param>
        /// <param name="jdata">JSON DATA</param>
        private async Task<bool> PutApiFrmUrl(string s,string jdata = "",int refund = 0)
        {
            bool rtn = false;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            try
            {
                // JSONデータ添付
                HttpContent content = new StringContent(jdata,Encoding.UTF8,"application/json");
                // HttpHeader編集
                var client = new HttpClient();
                // QRコード支払と返金処理の設定を行う
                if (refund == 0)
                {
                    // TimeOut
                    client.Timeout = TimeSpan.FromMilliseconds(pCPMTimeOut);
                }
                else
                {
                    // TimeOut
                    client.Timeout = TimeSpan.FromMilliseconds(pRefundTimeOut);
                }

                AddHttpHeader2(ref client);
                var res = await client.PutAsync(s, content);

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("PUT成功！");
                    var g = await res.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(g))
                    {
                        cpm = new MakeJsons.CpmRes();
                        cpm = JsonConvert.DeserializeObject<MakeJsons.CpmRes>(g);
                        // 確認のためアイアログ表示
                        string cpmres =
                            string.Format("ReturnCode:{0}", cpm.ReturnCode) + "\r\n" +
                            string.Format("ReturnMessage:{0}", cpm.ReturnMessage) + "\r\n" +
                            string.Format("MsgSummaryCode:{0}", cpm.MsgSummaryCode) + "\r\n" +
                            string.Format("MsgSummary:{0}", cpm.MsgSummary) + "\r\n" +
                            string.Format("Result.Partner_order_id:{0}", cpm.Result.Partner_order_id) + "\r\n" +
                            string.Format("Result.Currency:{0}", cpm.Result.Currency) + "\r\n" +
                            string.Format("Result.Order_id:{0}", cpm.Result.Order_id) + "\r\n" +
                            string.Format("Result.Return_code:{0}", cpm.Result.Return_code) + "\r\n" +
                            string.Format("Result.Result_code:{0}", cpm.Result.Result_code) + "\r\n" +
                            string.Format("Result.Create_time:{0}", cpm.Result.Create_time) + "\r\n" +
                            string.Format("Result.Total_fee:{0}", cpm.Result.Total_fee.ToString()) + "\r\n" +
                            string.Format("Result.Real_fee:{0}", cpm.Result.Real_fee.ToString()) + "\r\n" +
                            string.Format("Result.Channel:{0}", cpm.Result.Channel) + "\r\n" +
                            string.Format("Result.Pay_time:{0}", cpm.Result.Pay_time) + "\r\n" +
                            string.Format("Result.Order_body:{0}", cpm.Result.Order_body) + "\r\n" +
                            string.Format("BalanceAmount:{0}", cpm.BalanceAmount.ToString());

                        Console.WriteLine(cpmres, "帰ってきたjsonパラメタ");
                        // 処理正常でかつ支払完了時のみ支払確認処理をさせない
                        if ((cpm.ReturnCode == cReturnCode) && (cpm.Result.Result_code == cResult_Code_S))
                        {
                            rtn = true;
                            MessageBox.Show("支払が完了しました。", "QRコード支払");
                        }

                    }
                    else
                    {
                        MessageBox.Show("Null had come...", "帰ってきたjsonパラメタ");
                    }
                }
                else
                {
                    Console.WriteLine("PUT失敗！");    
                    var g = res.StatusCode.ToString();
                    MessageBox.Show(g, "PUT失敗のStatusCode");
                }
                // http client開放
                client.Dispose();

            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("あかんやん！: {0}", e.Message));
                Console.WriteLine(string.Format("なんでやねん！: {0}", e.InnerException));
                MessageBox.Show(string.Format("Exceptionが発生しましたよ。:\r\n{0} \r\ninner Ex:\r\n{1}", e.Message, e.InnerException),
                    "残念ながらPUTに失敗してます。");
            }

            return rtn;
        }

        // UNIXエポックを表すDateTimeオブジェクトを取得
        private static DateTime UNIX_EPOCH =
          new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>
        /// httpヘッダーを編集する
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private void AddHttpHeader(ref HttpRequestMessage request)
        {
            // ミリ秒(3桁)単位をUNIX時間に変換(13桁)
            var unixTimeStamp = (long)DateTime.Now.Subtract(UNIX_EPOCH).TotalMilliseconds;
            request.Headers.Add("X-LAKALA-Time", unixTimeStamp.ToString());
            // ランダム文字列(15桁---半角英数字<英字は大小文字可>)
            string rndm = GetRandomWords(15);
            request.Headers.Add("X-LAKALA-NonceStr", rndm);
            /* 
             * トークンハッシュ
             * ログインID 、ミリ秒で表される現在の時間、ランダム文字列、 認証キー を
             * “＆”で 連結した文字列 の sha256 ハッシュ値 の 16 進数表記文字列 を小文字に変換する。
             */
            string uID = "abcdefg001";      // for TEST
            string uAUTH = "auth001OK002";  // for TEST
            
            string s = GetTalknHash(uID, unixTimeStamp.ToString(), rndm, uAUTH);
            request.Headers.Add("X-LAKALA-Sign", s);
            /*
             * ログインID
             * 発行した端末ユーザ ID（加盟店管理画面にて作成）
             * ユーザ認証APIのログインIDと同一
             */
            request.Headers.Add("X-LAKALA-loginId", uID);
            /*
             * 端末識別番号(半角英数記号 空白可 可変256---但し記号は「-」のみ)
             */
            string uSN = "KONAMISPORTSCLUB-SINAGAWAHONTEN-0001";
            request.Headers.Add("X-LAKALA-serialNo", uSN);
        }

        private void AddHttpHeader2(ref HttpClient client)
        {
            // ミリ秒(3桁)単位をUNIX時間に変換(13桁)
            var unixTimeStamp = (long)DateTime.Now.Subtract(UNIX_EPOCH).TotalMilliseconds;
            client.DefaultRequestHeaders.Add("X-LAKALA-Time", unixTimeStamp.ToString());
            // ランダム文字列(15桁---半角英数字<英字は大小文字可>)
            string rndm = GetRandomWords(15);
            client.DefaultRequestHeaders.Add("X-LAKALA-NonceStr", rndm);
            /* 
             * トークンハッシュ
             * ログインID 、ミリ秒で表される現在の時間、ランダム文字列、 認証キー を
             * “＆”で 連結した文字列 の sha256 ハッシュ値 の 16 進数表記文字列 を小文字に変換する。
             */
            string uID = "abcdefg001";      // for TEST
            string uAUTH = "auth001OK002";  // for TEST

            string s = GetTalknHash(uID, unixTimeStamp.ToString(), rndm, uAUTH);
            client.DefaultRequestHeaders.Add("X-LAKALA-Sign", s);
            /*
             * ログインID
             * 発行した端末ユーザ ID（加盟店管理画面にて作成）
             * ユーザ認証APIのログインIDと同一
             */
            client.DefaultRequestHeaders.Add("X-LAKALA-loginId", uID);
            /*
             * 端末識別番号(半角英数記号 空白可 可変256---但し記号は「-」のみ)
             */
            string uSN = "KONAMISPORTSCLUB-SINAGAWAHONTEN-0001";
            client.DefaultRequestHeaders.Add("X-LAKALA-serialNo", uSN);

        }

        /// <summary>
        /// ランダム文字列生成用対象文字列
        /// ここに記号を入れれば英数字(大小文字)＋記号も含めた中から指定文字数のランダム文字が生成される
        /// </summary>
        const string RNDM_CHARS = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        /// <summary>
        /// ランダム文字列生成メソッド
        /// </summary>
        /// <param name="len">桁数</param>
        /// <param name="PW">対象文字列</param>
        /// <returns></returns>
        private string GetRandomWords(int len, string PW = RNDM_CHARS)
        {
            if (string.IsNullOrEmpty(PW)) PW = RNDM_CHARS;

            var r = new Random();
            return string.Join("", Enumerable.Range(0, len).Select(_ => RNDM_CHARS[r.Next(PW.Length)]));
        }
        /// <summary>
        /// 各文字列を「&」で連結した文字列の
        /// sha256ハッシュ値の16進数表記文字列を小文字に変換する
        /// </summary>
        /// <param name="uID">ユーザーID</param>
        /// <param name="uTM">現在時刻(ミリ秒)</param>
        /// <param name="uRNDM">ランダム文字列</param>
        /// <param name="uAKey">認証キー</param>
        /// <returns></returns>
        private string GetTalknHash(string uID, string uTM, string uRNDM, string uAKey)
        {
            string ret = string.Empty;
            const string amp = "&";
            // 文字列を「&」で結合
            var wSt = uID + amp + uTM + amp + uRNDM + amp + uAKey;
            // Unicodeエンコード
            byte[] vs = Encoding.UTF8.GetBytes(wSt);

            SHA256 hA256 = new SHA256CryptoServiceProvider();
            byte[] vs1 = hA256.ComputeHash(vs);
            ret = BitConverter.ToString(vs1).ToLower();
            Console.WriteLine(ret);
            return ret;
        }
        /// <summary>
        /// QRコード支払(CPM)API[PUT METHOD]用 jsonデータ生成
        /// </summary>
        /// <param name="jdata">送信jsonデータ</param>
        /// <returns></returns>
        private bool PutCPM(ref string jdata)
        {
            bool ret = true;
            try
            {
                // CPMリクエスト JSON定義
                var req = new MakeJsons.CpmReq();
                // Requestパラメタ設定
                req.Order_id = "11234567890123456789";
                req.SerialNo = "KONAMISPORTS CLUB HONTEN-4501001";
                req.Description = "テストの取引備考でざます";
                req.Price = 500;
                req.Auth_code = "0001002003004005006007008009";
                req.Currency = "JPY";
                req.Operator = "abcdefg001";
                // JSON シリアライズ(JSONフォーマットテキスト)
                var JsonData = JsonConvert.SerializeObject(req);
                jdata = JsonData;

                Console.WriteLine("CPM PUT JSON:{0}", JsonData);
            }
            catch
            {
                ret = false;
            }
            return ret;
        }
        /// <summary>
        /// GET METHOD PAULING
        /// 支払確認API実行
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetPauling()
        {
            // PAULING 間隔待機
            await Task.Delay(pPollTime);
            // 支払確認API実行
            return await GetApiFrmUrl(pUrl,0);
            
        }
    }

}

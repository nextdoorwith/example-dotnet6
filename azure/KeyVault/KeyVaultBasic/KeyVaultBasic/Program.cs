
// - Azure Key Vault secret client library for .NET
//   https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/keyvault/Azure.Security.KeyVault.Secrets
// - Azure Identity client
//   https://docs.microsoft.com/ja-JP/dotnet/api/overview/azure/identity-readme

// 次のパッケージが必要
// - Azure.Security.KeyVault.Secrets
// - Azure.Identity
// - System.Linq.Async

using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

// このサンプルで使用するシークレット名
const string SecretName = "mysecret06";

// キーコンテナURI(Azureポータルのキーコンテナの[概要]を参照)
const string KeyVaultUri = "https://deveval.vault.azure.net/";

// シークレット情報をコンソール出力するアクション
Action<string, KeyVaultSecret> ShowKey = (prefix, k)
    => Console.WriteLine($"{prefix}: {k.Value} " +
        $"({k.Properties.CreatedOn}, {k.Properties.Version}, " +
        $"{k.Properties.Tags.Count}, {k.Properties.ContentType})");

// シークレット操作の中心となるクライアントを作成
var client = new SecretClient(new Uri(KeyVaultUri), new DefaultAzureCredential());

// シークレットの一覧取得
// - 個々のシークレット値は含まれない
// - 返却型Pageable<T>の扱い方は次のURIを参照
//   https://docs.microsoft.com/ja-jp/dotnet/azure/sdk/pagination
Console.WriteLine($"=== secret list");
var propsPageable = client.GetPropertiesOfSecretsAsync();
var propsList = await propsPageable.ToListAsync(); // await foreachでも可
for (var i = 0; i < propsList.Count; i++)
    Console.WriteLine($"{i}: {propsList[i].Name}");

Console.WriteLine($"=== secret: {SecretName}");

// シークレットの作成(version1)
// - 同名シークレットがある場合は更新
// - Azure.Response<T>の自動型変換を利用するためにvarでなくKeyVaultSecretを指定
//   https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/core/Azure.Core
//   https://docs.microsoft.com/ja-jp/dotnet/csharp/language-reference/operators/user-defined-conversion-operators
//   https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/core/Azure.Core/src/ResponseOfT.cs
KeyVaultSecret s1 = await client.SetSecretAsync(SecretName, "secret1");
ShowKey("created", s1);

Thread.Sleep(1000); // 履歴上の更新順番が識別しやすいよう少々待機

// シークレットの更新(version2)
KeyVaultSecret s2 = await client.SetSecretAsync(SecretName, "secret2");
ShowKey("updated", s2);

// シークレットのプロパティの更新
// - プロパティはバージョンに紐づき、この更新はversion2のみで有効
// - プロパティを更新してもversionは変わらない
var u2Props = s2.Properties;
u2Props.ContentType = "text/plain";
u2Props.Tags["updater"] = "KeyVaultBasic";
await client.UpdateSecretPropertiesAsync(u2Props);

Thread.Sleep(1000);

// シークレットの更新(version3)
KeyVaultSecret s3 = await client.SetSecretAsync(SecretName, "secret3");
ShowKey("updated", s3);


// シークレットの取得(最新のversion3を取得)
KeyVaultSecret latest = await client.GetSecretAsync(SecretName);
var latestProps = latest.Properties;
ShowKey("latest ", latest);

// シークレットの履歴一覧の取得
var versPageable = client.GetPropertiesOfSecretVersionsAsync(SecretName);
var verList = await versPageable.ToListAsync();
var orderdVerList = verList
    .OrderByDescending(x => x.CreatedOn.Value.ToUniversalTime()).ToList();
for (var i = 0; i < orderdVerList.Count; i++)
{
    // バージョン指定でシークレットを取得
    KeyVaultSecret s = client.GetSecret(SecretName, orderdVerList[i].Version);
    ShowKey(string.Format("history[{0, 2}]", i), s);
}

// シークレットの論理削除
Console.WriteLine("deleting...");
var operation = await client.StartDeleteSecretAsync(SecretName);

// シークレットの物理削除(purge)
// - 物理削除(purge)や回復(recovery)を行う場合、論理削除の完了の待機が必要
// - アクセスポリシーの[特権シークレットの操作]の[削除]権限が必要
// - 権限がない場合、次の例外
//   Azure.RequestFailedException(0x8013150): 403 (Forbidden)
Console.WriteLine("wait deleting: start");
await operation.WaitForCompletionAsync();
Console.WriteLine("wait deleting: end");
await client.PurgeDeletedSecretAsync(SecretName);

using AesBasicExample;
using System.Security.Cryptography;
using System.Text;

Console.WriteLine("-----");
{
    using var aes = Aes.Create();
    aes.GenerateKey();
    aes.GenerateIV();
    var key = aes.Key;
    var iv = aes.IV;
    Console.WriteLine("{0,-10}: {1}({2})", "key", Convert.ToHexString(key), key.Length);
    Console.WriteLine("{0,-10}: {1}({2})", "iv", Convert.ToHexString(iv), iv.Length);

    var text = "テストデータ";
    var cleartext = Encoding.UTF8.GetBytes(text);
    Console.WriteLine("{0,-10}: {1}", "text", text);
    Console.WriteLine("{0,-10}: {1}({2})", "cleartext", Convert.ToHexString(cleartext), cleartext.Length);

    using var output = new MemoryStream();
    AesExample.Encrypt(output, cleartext, key, iv);
    var ciphertext = output.ToArray();
    Console.WriteLine("{0,-10}: {1}({2})", "ciphertext", Convert.ToHexString(ciphertext), ciphertext.Length);

    using var input = new MemoryStream(ciphertext);
    var decrypted = AesExample.Decrypt(input, key, iv);
    Console.WriteLine("{0,-10}: {1}({2})", "decrypted", Convert.ToHexString(decrypted), decrypted.Length);
}
Console.WriteLine("-----");
{
    using var aes = Aes.Create();
    aes.GenerateKey();
    aes.GenerateIV();
    var key = aes.Key;
    var iv = aes.IV;
    Console.WriteLine("{0,-10}: {1}({2})", "key", Convert.ToHexString(key), key.Length);
    Console.WriteLine("{0,-10}: {1}({2})", "iv", Convert.ToHexString(iv), iv.Length);

    var text = "テストデータ";
    var cleartext = Encoding.UTF8.GetBytes(text);
    Console.WriteLine("{0,-10}: {1}", "text", text);
    Console.WriteLine("{0,-10}: {1}({2})", "cleartext", Convert.ToHexString(cleartext), cleartext.Length);

    var ciphertext = AesExample.Encrypt(cleartext, key, iv);
    Console.WriteLine("{0,-10}: {1}({2})", "ciphertext", Convert.ToHexString(ciphertext), ciphertext.Length);

    var decrypted = AesExample.Decrypt(ciphertext, key, iv);
    Console.WriteLine("{0,-10}: {1}({2})", "decrypted", Convert.ToHexString(decrypted), decrypted.Length);
}
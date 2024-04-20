using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Text;


namespace HiveServer;
public class Security
{
    private const int SaltSize = 16; // 솔트의 바이트 크기
    private const int Iterations = 10000; // PBKDF2 반복 횟수
    private const int SubkeyLength = 20; // 해시된 비밀번호의 바이트 크기

    public static (string Salt, string HashedPassword) GenerateHashValue(string pw)
    {
        byte[] salt = new byte[SaltSize];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // PBKDF2 알고리즘을 사용하여 비밀번호 해싱
        string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: pw,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: Iterations,
            numBytesRequested: SubkeyLength));

        return (Convert.ToBase64String(salt), hashedPassword);
    }

    //비밀번호 검증하는 함수
    public static bool VerifyPassword(string password, string storedSalt, string storedHash)
    {
        Console.WriteLine("lejkdslfjklsdfj");
        // Base64 문자열을 바이트 배열로 변환
        byte[] salt = Convert.FromBase64String(storedSalt);
        byte[] expectedHash = Convert.FromBase64String(storedHash);

        // 입력된 비밀번호를 사용하여 해시 생성
        byte[] actualHash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: Iterations,
            numBytesRequested: SubkeyLength);

        // 계산된 해시와 저장된 해시 비교
        return ByteArraysEqual(actualHash, expectedHash);
    }


    public static string GenerateToken()
    {
        Guid tokenGuid = Guid.NewGuid();

        // GUID를 문자열로 변환하여 하이픈(-) 제거
        string token = tokenGuid.ToString("N");

        return token;
    }

    // 두 바이트 배열 비교
    private static bool ByteArraysEqual(byte[] array1, byte[] array2)
    {
        if (array1 == null || array2 == null || array1.Length != array2.Length)
        {
            return false;
        }

        for (int i = 0; i < array1.Length; i++)
        {
            if (array1[i] != array2[i])
            {
                return false;
            }
        }

        return true;
    }




}
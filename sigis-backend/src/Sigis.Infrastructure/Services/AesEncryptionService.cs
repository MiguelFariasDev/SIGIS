using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Sigis.Infrastructure.Services;

/// <summary>
/// Implementação de <see cref="IEncryptionService"/> baseada em AES-GCM
/// (autenticado) com nonce <b>determinístico</b>, com a chave derivada de
/// <c>Encryption:Key</c> (appsettings) ou da variável de ambiente
/// <c>SIGIS_ENCRYPTION_KEY</c>.
/// </summary>
/// <remarks>
/// <para>
/// Decisão de hackathon (ver <c>claude.md</c>, seção LGPD): criptografia na
/// camada de aplicação via <see cref="AesGcm"/> em vez de <c>pgcrypto</c>
/// transparente no PostgreSQL — mais simples de configurar e testar em 20h,
/// sem exigir funções SQL customizadas nas migrations.
/// </para>
/// <para>
/// <b>Por que o nonce é determinístico (HMAC-SHA256 do texto em claro) em
/// vez de aleatório:</b> CNS e CPF precisam permanecer pesquisáveis por
/// igualdade (<c>IPersonRepository.GetByCnsAsync/GetByCpfAsync</c>) e ter
/// unicidade garantida por índice único no banco (RF02/RN — "já existe
/// pessoa com este CNS/CPF"). Com nonce aleatório, o mesmo CNS geraria um
/// ciphertext diferente a cada gravação, tornando impossível comparar por
/// igualdade ou impor unicidade no banco. Usar um nonce derivado
/// deterministicamente do texto em claro (com uma subchave própria, distinta
/// da subchave de cifragem) faz com que o mesmo valor sempre produza o mesmo
/// ciphertext, preservando busca e unicidade. O tradeoff aceito é a perda de
/// semântica de segurança "probabilística": um invasor com acesso ao banco
/// (mas sem a chave) pode perceber que duas linhas compartilham o mesmo CNS,
/// mas não consegue recuperar o valor em claro sem a chave — esse é o
/// padrão conhecido como "criptografia determinística"/"blind indexing",
/// aceitável para identificadores de baixa entropia que precisam de busca
/// exata. Colunas que não exigem busca por igualdade (ex.: nenhuma neste
/// projeto) poderiam usar nonce aleatório para segurança probabilística plena.
/// </para>
/// <para>
/// A chave configurada pode ter qualquer tamanho: é normalizada via
/// SHA-256 antes do uso, com subchaves distintas derivadas para cifragem e
/// para o nonce. Em produção, a chave deve vir exclusivamente de uma
/// variável de ambiente/cofre de segredos, nunca do arquivo de configuração
/// versionado.
/// </para>
/// </remarks>
public sealed class AesEncryptionService : IEncryptionService
{
    private const int NonceSizeInBytes = 12;
    private const int TagSizeInBytes = 16;

    private readonly byte[] _encryptionKey;
    private readonly byte[] _nonceKey;

    /// <summary>
    /// Cria o serviço de criptografia, lendo a chave de <c>Encryption:Key</c>
    /// ou da variável de ambiente <c>SIGIS_ENCRYPTION_KEY</c>.
    /// </summary>
    /// <param name="configuration">Configuração da aplicação.</param>
    /// <exception cref="InvalidOperationException">
    /// Lançada quando nenhuma chave de criptografia está configurada —
    /// indica falha de configuração da infraestrutura, não regra de negócio.
    /// </exception>
    public AesEncryptionService(IConfiguration configuration)
    {
        var rawKey = configuration["Encryption:Key"]
            ?? Environment.GetEnvironmentVariable("SIGIS_ENCRYPTION_KEY")
            ?? throw new InvalidOperationException(
                "Chave de criptografia não configurada. Defina 'Encryption:Key' no appsettings ou a variável de ambiente SIGIS_ENCRYPTION_KEY.");

        var rawKeyBytes = Encoding.UTF8.GetBytes(rawKey);
        _encryptionKey = SHA256.HashData([.. rawKeyBytes, .. "sigis:enc"u8]);
        _nonceKey = SHA256.HashData([.. rawKeyBytes, .. "sigis:nonce"u8]);
    }

    /// <inheritdoc />
    public string Encrypt(string plainText)
    {
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var nonce = ComputeDeterministicNonce(plainBytes);
        var cipherBytes = new byte[plainBytes.Length];
        var tag = new byte[TagSizeInBytes];

        using var aesGcm = new AesGcm(_encryptionKey, TagSizeInBytes);
        aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);

        var payload = new byte[NonceSizeInBytes + TagSizeInBytes + cipherBytes.Length];
        Buffer.BlockCopy(nonce, 0, payload, 0, NonceSizeInBytes);
        Buffer.BlockCopy(tag, 0, payload, NonceSizeInBytes, TagSizeInBytes);
        Buffer.BlockCopy(cipherBytes, 0, payload, NonceSizeInBytes + TagSizeInBytes, cipherBytes.Length);

        return Convert.ToBase64String(payload);
    }

    /// <inheritdoc />
    public string Decrypt(string cipherText)
    {
        var payload = Convert.FromBase64String(cipherText);

        var nonce = payload.AsSpan(0, NonceSizeInBytes);
        var tag = payload.AsSpan(NonceSizeInBytes, TagSizeInBytes);
        var cipherBytes = payload.AsSpan(NonceSizeInBytes + TagSizeInBytes);
        var plainBytes = new byte[cipherBytes.Length];

        using var aesGcm = new AesGcm(_encryptionKey, TagSizeInBytes);
        aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);

        return Encoding.UTF8.GetString(plainBytes);
    }

    private byte[] ComputeDeterministicNonce(byte[] plainBytes)
    {
        var fullHash = HMACSHA256.HashData(_nonceKey, plainBytes);
        return fullHash[..NonceSizeInBytes];
    }
}

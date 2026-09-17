import 'package:flutter_test/flutter_test.dart';
import 'package:sigis_mobile/core/utils/validators.dart';

void main() {
  group('Validators.isValidCpf', () {
    test('aceita CPF valido formatado', () {
      expect(Validators.isValidCpf('111.444.777-35'), isTrue);
    });

    test('rejeita CPF com todos os digitos iguais', () {
      expect(Validators.isValidCpf('111.111.111-11'), isFalse);
    });

    test('rejeita CPF com quantidade errada de digitos', () {
      expect(Validators.isValidCpf('123'), isFalse);
    });
  });

  group('Validators.isValidCns', () {
    test('aceita CNS com 15 digitos', () {
      expect(Validators.isValidCns('123456789012345'), isTrue);
    });

    test('rejeita CNS com menos de 15 digitos', () {
      expect(Validators.isValidCns('12345'), isFalse);
    });
  });

  group('Validators.isValidPhone', () {
    test('aceita telefone com DDD e 9 digitos', () {
      expect(Validators.isValidPhone('(85) 99999-8888'), isTrue);
    });

    test('rejeita telefone sem DDD', () {
      expect(Validators.isValidPhone('999998888'), isFalse);
    });
  });

  group('Validators.isNotEmpty', () {
    test('rejeita string apenas com espacos', () {
      expect(Validators.isNotEmpty('   '), isFalse);
    });

    test('aceita string com conteudo', () {
      expect(Validators.isNotEmpty('Ana'), isTrue);
    });
  });
}

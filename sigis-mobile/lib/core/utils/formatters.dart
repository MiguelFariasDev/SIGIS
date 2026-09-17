import 'package:intl/intl.dart';

/// Funcoes de formatacao de datas, numeros e documentos em pt-BR.
abstract final class Formatters {
  static final DateFormat _dateFormat = DateFormat('dd/MM/yyyy', 'pt_BR');
  static final DateFormat _dateTimeFormat =
      DateFormat('dd/MM/yyyy HH:mm', 'pt_BR');
  static final DateFormat _timeFormat = DateFormat('HH:mm', 'pt_BR');

  /// Formata [date] como `dd/MM/yyyy`.
  static String date(DateTime date) => _dateFormat.format(date);

  /// Formata [dateTime] como `dd/MM/yyyy HH:mm`.
  static String dateTime(DateTime dateTime) =>
      _dateTimeFormat.format(dateTime);

  /// Formata [dateTime] como `HH:mm`.
  static String time(DateTime dateTime) => _timeFormat.format(dateTime);

  /// Calcula a idade em anos completos a partir de [birthDate].
  static int age(DateTime birthDate) {
    final now = DateTime.now();
    var years = now.year - birthDate.year;
    final hasNotHadBirthdayThisYear = now.month < birthDate.month ||
        (now.month == birthDate.month && now.day < birthDate.day);
    if (hasNotHadBirthdayThisYear) {
      years -= 1;
    }
    return years;
  }

  /// Formata a idade de [birthDate] como texto, ex.: "8 anos", ou "—"
  /// quando a data de nascimento nao estiver disponivel.
  static String ageLabel(DateTime? birthDate) {
    if (birthDate == null) {
      return '—';
    }
    final years = age(birthDate);
    return years == 1 ? '1 ano' : '$years anos';
  }

  /// Formata um tempo de espera em minutos como texto legivel, ex.:
  /// "1h 20min" ou "35min".
  static String waitingTime(Duration duration) {
    final hours = duration.inHours;
    final minutes = duration.inMinutes.remainder(60);
    if (hours <= 0) {
      return '${minutes}min';
    }
    return '${hours}h ${minutes}min';
  }

  /// Formata um CPF (apenas digitos) como `000.000.000-00`.
  static String cpf(String digitsOnly) {
    final digits = digitsOnly.replaceAll(RegExp(r'\D'), '');
    if (digits.length != 11) {
      return digitsOnly;
    }
    return '${digits.substring(0, 3)}.${digits.substring(3, 6)}.'
        '${digits.substring(6, 9)}-${digits.substring(9, 11)}';
  }
}

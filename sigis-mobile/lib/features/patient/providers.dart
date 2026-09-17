import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigis_mobile/data/models/patient.dart';
import 'package:sigis_mobile/features/patient/presentation/patient_detail_controller.dart';

/// Provider do controller de resumo do paciente (F04), parametrizado
/// pelo identificador da pessoa.
final patientDetailControllerProvider = AsyncNotifierProvider.autoDispose
    .family<PatientDetailController, Patient, String>(
  PatientDetailController.new,
);

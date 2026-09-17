import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:sigis_mobile/core/errors/app_error.dart';
import 'package:sigis_mobile/core/utils/formatters.dart';
import 'package:sigis_mobile/features/search/providers.dart';
import 'package:sigis_mobile/shared/widgets/empty_state.dart';
import 'package:sigis_mobile/shared/widgets/error_banner.dart';

/// Tela de busca rapida de pacientes (F03).
///
/// Busca por nome, CNS ou CPF, com debounce de 300ms e combinacao de
/// resultados locais (cache) e remotos.
class SearchPage extends ConsumerStatefulWidget {
  /// Cria a tela de busca rapida.
  const SearchPage({super.key});

  @override
  ConsumerState<SearchPage> createState() => _SearchPageState();
}

class _SearchPageState extends ConsumerState<SearchPage> {
  final _controller = TextEditingController();
  final _focusNode = FocusNode();

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _focusNode.requestFocus();
    });
  }

  @override
  void dispose() {
    _controller.dispose();
    _focusNode.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final resultsState = ref.watch(searchControllerProvider);

    return Scaffold(
      appBar: AppBar(title: const Text('Buscar paciente')),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.all(16),
            child: TextField(
              controller: _controller,
              focusNode: _focusNode,
              autofocus: true,
              decoration: const InputDecoration(
                hintText: 'Nome, CNS ou CPF',
                prefixIcon: Icon(Icons.search),
              ),
              onChanged: (value) =>
                  ref.read(searchControllerProvider.notifier).onTermChanged(value),
            ),
          ),
          Expanded(
            child: resultsState.when(
              data: (patients) {
                if (_controller.text.trim().length < 2) {
                  return const EmptyState(
                    message: 'Digite ao menos 2 caracteres para buscar.',
                    icon: Icons.search,
                  );
                }
                if (patients.isEmpty) {
                  return const EmptyState(
                    message: 'Nenhum paciente encontrado.',
                    icon: Icons.person_search_outlined,
                  );
                }
                return ListView.separated(
                  itemCount: patients.length,
                  separatorBuilder: (_, _) => const Divider(height: 1),
                  itemBuilder: (context, index) {
                    final patient = patients[index];
                    return ListTile(
                      title: Text(patient.fullName),
                      subtitle: Text(
                        '${Formatters.date(patient.birthDate)}'
                        '${patient.cns != null ? ' • CNS ${patient.cns}' : ''}',
                      ),
                      onTap: () => context.push('/patient/${patient.id}'),
                    );
                  },
                );
              },
              loading: () => const Center(child: CircularProgressIndicator()),
              error: (error, stackTrace) => Center(
                child: ErrorBanner(
                  message: error is AppError
                      ? error.message
                      : 'Nao foi possivel buscar pacientes.',
                  isWarning: error is AppError && error.code == 'PATIENT_003',
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

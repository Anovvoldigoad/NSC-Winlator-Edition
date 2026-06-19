import 'package:flutter/material.dart';
import 'services/mod_service.dart';
import 'models/mod.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  final modService = ModService();
  await modService.initialize();
  runApp(MyApp(modService: modService));
}

class MyApp extends StatelessWidget {
  final ModService modService;
  
  const MyApp({required this.modService});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'NSC Mod Manager',
      theme: ThemeData(
        useMaterial3: true,
        brightness: Brightness.dark,
        colorSchemeSeed: Colors.blue,
      ),
      home: ModManagerScreen(modService: modService),
    );
  }
}

class ModManagerScreen extends StatefulWidget {
  final ModService modService;
  
  const ModManagerScreen({required this.modService});

  @override
  State<ModManagerScreen> createState() => _ModManagerScreenState();
}

class _ModManagerScreenState extends State<ModManagerScreen> {
  late Future<List<Mod>> modsFuture;

  @override
  void initState() {
    super.initState();
    modsFuture = widget.modService.listMods();
  }

  void refreshMods() {
    setState(() {
      modsFuture = widget.modService.listMods();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('NSC Mod Manager'),
        centerTitle: true,
        elevation: 0,
      ),
      body: FutureBuilder<List<Mod>>(
        future: modsFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }

          if (snapshot.hasError) {
            return Center(
              child: Text('Error: ${snapshot.error}'),
            );
          }

          final mods = snapshot.data ?? [];
          
          if (mods.isEmpty) {
            return const Center(
              child: Text('No mods installed\n\nLaunch CLI to install mods'),
              textAlign: TextAlign.center,
            );
          }

          return ListView.builder(
            padding: const EdgeInsets.all(8),
            itemCount: mods.length,
            itemBuilder: (context, index) {
              final mod = mods[index];
              return Card(
                child: ListTile(
                  title: Text(mod.name),
                  subtitle: Text('Installed'),
                  trailing: IconButton(
                    icon: const Icon(Icons.delete, color: Colors.red),
                    onPressed: () => showDeleteDialog(mod),
                  ),
                ),
              );
            },
          );
        },
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: refreshMods,
        tooltip: 'Refresh',
        child: const Icon(Icons.refresh),
      ),
    );
  }

  void showDeleteDialog(Mod mod) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Remove Mod?'),
        content: Text('Remove ${mod.name}?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () async {
              await widget.modService.removeMod(mod.name);
              Navigator.pop(context);
              refreshMods();
            },
            child: const Text('Remove', style: TextStyle(color: Colors.red)),
          ),
        ],
      ),
    );
  }
}

import 'dart:io';
import 'package:path_provider/path_provider.dart';
import '../models/mod.dart';

class ModService {
  late String modsFolder;

  Future<void> initialize() async {
    if (Platform.isAndroid) {
      final appDir = await getExternalStorageDirectory();
      modsFolder = '${appDir?.path}/NSC.Winlator/Mods';
    } else {
      final appDir = await getApplicationDocumentsDirectory();
      modsFolder = '${appDir.path}/NSC.Winlator/Mods';
    }
    
    final dir = Directory(modsFolder);
    if (!dir.existsSync()) {
      dir.createSync(recursive: true);
    }
  }

  Future<List<Mod>> listMods() async {
    final dir = Directory(modsFolder);
    if (!dir.existsSync()) return [];

    final mods = <Mod>[];
    try {
      for (var entity in dir.listSync()) {
        if (entity is Directory) {
          mods.add(Mod(name: entity.path.split('/').last, path: entity.path));
        }
      }
    } catch (e) {
      print('Error listing mods: $e');
    }
    return mods;
  }

  Future<bool> removeMod(String modName) async {
    try {
      final modDir = Directory('$modsFolder/$modName');
      if (modDir.existsSync()) {
        modDir.deleteSync(recursive: true);
        return true;
      }
    } catch (e) {
      print('Error removing mod: $e');
    }
    return false;
  }
}

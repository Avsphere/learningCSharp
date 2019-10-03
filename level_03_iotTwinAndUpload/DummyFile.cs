using System;
using System.IO;


namespace level_3_iotTwinAndUpload {
    public static class DummyFile {
        private static string dummyFilePath = @"./dummy.txt";
        public static string createDummyFile(){
            if ( !File.Exists(dummyFilePath) ){
                using (StreamWriter streamWriter = File.CreateText(dummyFilePath) ) {
                    streamWriter.WriteLine("This is a dummy file" );
                }
            } else {
                removeDummyFile();
                return createDummyFile();
            }
            return dummyFilePath;
        }

        public static void removeDummyFile(){
            if ( !File.Exists(dummyFilePath) ) {
                throw new InvalidOperationException("File not found");
            } else {
                File.Delete(dummyFilePath);
            }
        }
    }


}
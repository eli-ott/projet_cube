namespace Cube.Tests {
    public static class API {
        public static void RunTests() {
            TestEnsureThreadSafety_SuccessfulRequest();
            TestEnsureThreadSafety_ErrorWhenAlreadyQuerying();
        } // void ..


        private static void TestEnsureThreadSafety_SuccessfulRequest() {

            Program.IsQuerying       = false;
            ApiResponse<int>? result = Program.EnsureThreadSafety(args => ApiResponse<int>.Success(42));

            ConsoleLogger.LogAssert(false, Program.IsQuerying, "IsQuerying devrait être égal à `false` après la requête !");
            ConsoleLogger.LogAssert(42,    result.donnee,      "La requête doit renvoyer les données attendues !");
            
        } // void ..


        private static void TestEnsureThreadSafety_ErrorWhenAlreadyQuerying() {

            Program.IsQuerying      = true;
            ApiResponse<int> result = Program.EnsureThreadSafety(args => ApiResponse<int>.Success(42));
            ConsoleLogger.LogAssert(false, result.reussite, "La reqête doit échouer si l'API est déjà en train d'en gérer une !");

        } // void ..
    } // class ..
} // namespace ..

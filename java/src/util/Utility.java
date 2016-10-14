/**
 * Copyright Microsoft Corporation
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package util;

/**
 * A class which provides utility methods
 * 
 */
public final class Utility {
    static {
        // Uncomment the following to use Fiddler
        // System.setProperty("http.proxyHost", "localhost");
        // System.setProperty("http.proxyPort", "8888");
    }

    /**
     * MODIFY THIS!
     * 
     * Stores the storage connection string.
     */
    public static final String storageConnectionString = "DefaultEndpointsProtocol=https;"
            + "AccountName=;"
            + "AccountKey=";

    /**
     * You only need to modify the following values if you want to run the
     * KeyVault Encryption samples. Otherwise, leave empty.
     */
    static final String vaultURL = "";
    static final String AuthClientId = "";
    static final String AuthClientSecret = "";

    /**
     * Optional. Modify this if you want to run the KeyVaultGettingStarted
     * sample.
     */
    public static final String keyVaultKeyID = null;

    /**
     * Prints out the sample start information .
     */
    public static void printSampleStartInfo(String sampleName) {
        System.out.println(String.format(
                "The Azure storage client library sample %s starting...",
                sampleName));
    }

    /**
     * Prints out the sample complete information .
     */
    public static void printSampleCompleteInfo(String sampleName) {
        System.out.println(String.format(
                "The Azure storage client library sample %s completed.",
                sampleName));
    }
}

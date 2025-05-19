//
// Copyright (c) Microsoft. All rights reserved.
// To learn more, please visit the documentation - Quickstart: Azure Content Safety: https://aka.ms/acsstudiodoc
//

package com.microsoft.cognitiveservices;
import okhttp3.*;

public class ContentSafetyDetectProtectedMaterialForCodeSampleCode {

    public static void main(String[] args) throws Exception {
        // Replace with your own subscription_key and endpoint
        String subscriptionKey = "your_subscription_key";
        String endpoint = "https://your_resource_name.cognitiveservices.azure.com";
        String aadToken = "your_aad_token";

        // The code to be analyzed
        String codeToAnalyze = "test-code";

        // Set up the API request
        String apiUrl = endpoint + "/contentsafety/text:detectProtectedMaterialForCode?api-version=2024-09-15-preview";

        String requestBody = "{\"code\": \"" + codeToAnalyze + "\"}";

        // Build request
        Headers.Builder headersBuilder = new Headers.Builder();
        //Choose to use key authorization or AAD token authorization
        headersBuilder.add("Ocp-Apim-Subscription-Key", subscriptionKey);
        headersBuilder.add("Authorization", aadToken);
        Request request = new Request.Builder()
                .url(apiUrl)
                .headers(headersBuilder.build())
                .post(RequestBody.create(requestBody, okhttp3.MediaType.parse("application/json; charset=utf-8")))
                .build();

        OkHttpClient client = new OkHttpClient();

        // Send request and get result
        try (Response response = client.newCall(request).execute()) {
            if (response.body() == null) {
                throw new Exception( "Response body is null.");
            }
            String responseText = response.body().string();
            System.out.println(responseText);

            if (!response.isSuccessful()) {
                    throw new Exception(String.valueOf(response.code()));
            }
        }
    }
}
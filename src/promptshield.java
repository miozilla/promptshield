//
// Copyright (c) Microsoft. All rights reserved.
// To learn more, please visit the documentation - Quickstart: Azure Content Safety: https://aka.ms/acsstudiodoc
//

package com.microsoft.cognitiveservices;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.Data;
import okhttp3.*;

public class shieldPromptSampleCode {
    public static void main(String[] args) throws Exception {
        // Replace with your own subscription_key and endpoint
        String subscriptionKey = "<subscription_key>";
        String endpoint = "<endpoint>";
        String apiVersion = "2024-09-01";

        // Set up the API request
        String apiUrl = String.format("%s/contentsafety/text:shieldPrompt?api-version=%s", endpoint, apiVersion);

        // Set the userPrompt
        String userPrompt = "user_prompt";

        // Set documents to be analyzed
        String [] documents = {
            "<documents>",
            "<another_documents>"
        };

        // Generate requestBody according to task.
        ShieldPromptRequestBody shieldRequest = new ShieldPromptRequestBody();
        shieldRequest.setDocuments(documents);
        shieldRequest.setUserPrompt(userPrompt);

        ObjectMapper mapper = new ObjectMapper();
        String requestBody = mapper.writeValueAsString(shieldRequest);

        // Build the request body
        Headers.Builder headersBuilder = new Headers.Builder();
        headersBuilder.add("Ocp-Apim-Subscription-Key", subscriptionKey);
        Request request = new Request.Builder()
                .url(apiUrl)
                .headers(headersBuilder.build())
                .post(RequestBody.create(requestBody, MediaType.parse("application/json; charset=utf-8")))
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

    @JsonIgnoreProperties(ignoreUnknown = true)
    @Data
    public static class ShieldPromptRequestBody {
        /// <summary>
        /// User prompt to be analyzed
        /// </summary>
        private String userPrompt = "";

        /// <summary>
        /// Documents to be analyzed
        /// </summary>
        private String[] documents;
    }
}

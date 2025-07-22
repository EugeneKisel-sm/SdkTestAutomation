package com.conductor.sdkwrapper;

import com.fasterxml.jackson.annotation.JsonProperty;

public class SdkResponse {
    @JsonProperty("statusCode")
    private int statusCode;
    
    @JsonProperty("success")
    private boolean success;
    
    @JsonProperty("data")
    private Object data;
    
    @JsonProperty("content")
    private String content;
    
    @JsonProperty("errorMessage")
    private String errorMessage;
    
    public SdkResponse() {}
    
    public SdkResponse(boolean success, String errorMessage, int statusCode) {
        this.success = success;
        this.errorMessage = errorMessage;
        this.statusCode = statusCode;
    }
    
    public SdkResponse(boolean success, String content, int statusCode, Object data) {
        this.success = success;
        this.content = content;
        this.statusCode = statusCode;
        this.data = data;
    }
    
    public int getStatusCode() {
        return statusCode;
    }
    
    public void setStatusCode(int statusCode) {
        this.statusCode = statusCode;
    }
    
    public boolean isSuccess() {
        return success;
    }
    
    public void setSuccess(boolean success) {
        this.success = success;
    }
    
    public Object getData() {
        return data;
    }
    
    public void setData(Object data) {
        this.data = data;
    }
    
    public String getContent() {
        return content;
    }
    
    public void setContent(String content) {
        this.content = content;
    }
    
    public String getErrorMessage() {
        return errorMessage;
    }
    
    public void setErrorMessage(String errorMessage) {
        this.errorMessage = errorMessage;
    }
} 
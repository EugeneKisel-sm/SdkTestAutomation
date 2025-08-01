package com.sdktestautomation.utils;

import com.sdktestautomation.models.SdkResponse;

/**
 * Centralized error handling for Java CLI operations
 */
public class ErrorHandler {
    
    public enum ErrorType {
        VALIDATION,
        NETWORK,
        BUSINESS_LOGIC,
        SYSTEM,
        UNKNOWN
    }
    
    /**
     * Handle exceptions and create appropriate error responses
     */
    public static SdkResponse handleException(Exception e, String operation) {
        ErrorType errorType = determineErrorType(e);
        int statusCode = getStatusCode(errorType);
        String message = buildErrorMessage(e, operation, errorType);
        
        return SdkResponse.createError(statusCode, message);
    }
    
    /**
     * Handle validation errors
     */
    public static SdkResponse handleValidationError(String message) {
        return SdkResponse.createError(400, "Validation error: " + message);
    }
    
    /**
     * Handle network errors
     */
    public static SdkResponse handleNetworkError(String message) {
        return SdkResponse.createError(503, "Network error: " + message);
    }
    
    /**
     * Handle business logic errors
     */
    public static SdkResponse handleBusinessError(String message) {
        return SdkResponse.createError(400, "Business logic error: " + message);
    }
    
    private static ErrorType determineErrorType(Exception e) {
        String message = e.getMessage().toLowerCase();
        String className = e.getClass().getSimpleName();
        
        if (className.contains("IllegalArgumentException") || 
            className.contains("Validation") ||
            message.contains("validation")) {
            return ErrorType.VALIDATION;
        }
        
        if (className.contains("ConnectException") || 
            className.contains("UnknownHostException") ||
            className.contains("SocketTimeoutException") ||
            message.contains("connection") ||
            message.contains("network")) {
            return ErrorType.NETWORK;
        }
        
        if (className.contains("BusinessException") ||
            message.contains("business") ||
            message.contains("logic")) {
            return ErrorType.BUSINESS_LOGIC;
        }
        
        return ErrorType.UNKNOWN;
    }
    
    private static int getStatusCode(ErrorType errorType) {
        return switch (errorType) {
            case VALIDATION -> 400;
            case NETWORK -> 503;
            case BUSINESS_LOGIC -> 400;
            case SYSTEM, UNKNOWN -> 500;
        };
    }
    
    private static String buildErrorMessage(Exception e, String operation, ErrorType errorType) {
        String baseMessage = String.format("Operation '%s' failed", operation);
        String errorTypeMessage = switch (errorType) {
            case VALIDATION -> "Validation error";
            case NETWORK -> "Network connectivity error";
            case BUSINESS_LOGIC -> "Business logic error";
            case SYSTEM -> "System error";
            case UNKNOWN -> "Unknown error";
        };
        
        return String.format("%s. Type: %s. Details: %s", baseMessage, errorTypeMessage, e.getMessage());
    }
} 
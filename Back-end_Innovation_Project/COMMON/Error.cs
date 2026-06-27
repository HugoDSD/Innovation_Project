namespace Back_end_Innovation_Project.COMMON;

public enum ErrorCode
{
    // Client errors (400+)
    VALIDATION_FAILED,
    NOT_FOUND,
    ALREADY_EXISTS,

    // Security errors (401/403)
    UNAUTHORIZED,
    FORBIDDEN,

    // Server errors (500)
    DATABASE_UNAVAILABLE,
    INTERNAL_ERROR,      // Error ONLY in the application back-end

    // Database error
    DATABASE_ERROR,     // Error ONLY in the database

    // Time/Size errors
    TIMEOUT,
    TOO_MANY_REQUESTS,
    PAYLOAD_TOO_LARGE
}
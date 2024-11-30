export function validateEmail(value) {
    const isFieldValid =
      value.includes("@") && value.length >= 3 && value.includes(".");

    return {
      isFieldValid,
      message: isFieldValid ? "" : "Invalid email address",
    };
  }

  export function validateName(value, identifier) {
    const isFieldValid = value?.trim().length > 0;

    return {
      isFieldValid,
      message: isFieldValid
        ? ""
        : `${capitiliseFirstLetter(identifier)} cannot be empty`,
    };
  }

  export function capitiliseFirstLetter(string) {
    if (string.length === 0) {
      return string;
    }

    return string.charAt(0).toUpperCase() + string.slice(1).toLowerCase();
  }

  export function validatePassword(value) {
    // min 8 chars
    if (value.length < 8) {
      return {
        isFieldValid: false,
        message: "Password must be at least 8 characters",
      };
    }

    // password must contain at least one digit
    if (!/\d/.test(value)) {
      return {
        isFieldValid: false,
        message: "Password must contain at least one digit.",
      };
    }

    // at least one lowercase letter
    if (!/[a-z]/.test(value)) {
      return {
        isFieldValid: false,
        message: "Password must contain at least one lowercase letter.",
      };
    }

    // at least one special character
    if (!/[^a-zA-Z0-9]/.test(value)) {
      return {
        isFieldValid: false,
        message: "Password must contain at least one special character.",
      };
    }

    // Check if password contains at least one uppercase letter
    if (!/[A-Z]/.test(value)) {
      return {
        isFieldValud: false,
        message: "Password must contain at least one uppercase letter.",
      };
    }

    const uniqueChars = new Set(value);
    if (uniqueChars.size < 1) {
      return {
        isFieldValid: false,
        message: "Password must contain at least one unique character.",
      };
    }

    // all validation passed at this point
    return {
      isFieldValid: true,
      message: "",
    };
  }
import { useState, useCallback } from 'react'

export interface ValidationErrors {
  [key: string]: string[]
}

export interface UseValidationState {
  errors: ValidationErrors
  hasErrors: boolean
}

export interface UseValidationActions {
  validateEmail: (email: string, fieldName?: string) => boolean
  validateRequired: (value: string | null | undefined, fieldName: string) => boolean
  validatePositive: (value: number, fieldName: string) => boolean
  validatePercentage: (value: number, fieldName: string) => boolean
  validateMinLength: (value: string, minLength: number, fieldName: string) => boolean
  validateMaxLength: (value: string, maxLength: number, fieldName: string) => boolean
  setError: (fieldName: string, errors: string[]) => void
  clearError: (fieldName: string) => void
  clearAllErrors: () => void
  validateForm: (data: Record<string, any>, schema: ValidationSchema) => boolean
}

export interface ValidationSchema {
  [fieldName: string]: ValidationRule[]
}

export interface ValidationRule {
  type: 'required' | 'email' | 'positive' | 'percentage' | 'minLength' | 'maxLength'
  minLength?: number
  maxLength?: number
  message?: string
}

export function useValidation() {
  const [errors, setErrors] = useState<ValidationErrors>({})

  const hasErrors = Object.keys(errors).length > 0

  const validateEmail = useCallback((email: string, fieldName = 'email'): boolean => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    const isValid = emailRegex.test(email)

    if (!isValid) {
      setErrors((prev) => ({
        ...prev,
        [fieldName]: ['Invalid email format'],
      }))
    } else {
      setErrors((prev) => {
        const updated = { ...prev }
        delete updated[fieldName]
        return updated
      })
    }

    return isValid
  }, [])

  const validateRequired = useCallback((value: string | null | undefined, fieldName: string): boolean => {
    const isValid = value !== null && value !== undefined && value.toString().trim() !== ''

    if (!isValid) {
      setErrors((prev) => ({
        ...prev,
        [fieldName]: [`${fieldName} is required`],
      }))
    } else {
      setErrors((prev) => {
        const updated = { ...prev }
        delete updated[fieldName]
        return updated
      })
    }

    return isValid
  }, [])

  const validatePositive = useCallback((value: number, fieldName: string): boolean => {
    const isValid = value > 0

    if (!isValid) {
      setErrors((prev) => ({
        ...prev,
        [fieldName]: [`${fieldName} must be greater than 0`],
      }))
    } else {
      setErrors((prev) => {
        const updated = { ...prev }
        delete updated[fieldName]
        return updated
      })
    }

    return isValid
  }, [])

  const validatePercentage = useCallback((value: number, fieldName: string): boolean => {
    const isValid = value >= 0 && value <= 1

    if (!isValid) {
      setErrors((prev) => ({
        ...prev,
        [fieldName]: [`${fieldName} must be between 0 and 1 (0-100%)`],
      }))
    } else {
      setErrors((prev) => {
        const updated = { ...prev }
        delete updated[fieldName]
        return updated
      })
    }

    return isValid
  }, [])

  const validateMinLength = useCallback((value: string, minLength: number, fieldName: string): boolean => {
    const isValid = value.length >= minLength

    if (!isValid) {
      setErrors((prev) => ({
        ...prev,
        [fieldName]: [`${fieldName} must be at least ${minLength} characters`],
      }))
    } else {
      setErrors((prev) => {
        const updated = { ...prev }
        delete updated[fieldName]
        return updated
      })
    }

    return isValid
  }, [])

  const validateMaxLength = useCallback((value: string, maxLength: number, fieldName: string): boolean => {
    const isValid = value.length <= maxLength

    if (!isValid) {
      setErrors((prev) => ({
        ...prev,
        [fieldName]: [`${fieldName} must not exceed ${maxLength} characters`],
      }))
    } else {
      setErrors((prev) => {
        const updated = { ...prev }
        delete updated[fieldName]
        return updated
      })
    }

    return isValid
  }, [])

  const setError = useCallback((fieldName: string, errorMessages: string[]) => {
    setErrors((prev) => ({
      ...prev,
      [fieldName]: errorMessages,
    }))
  }, [])

  const clearError = useCallback((fieldName: string) => {
    setErrors((prev) => {
      const updated = { ...prev }
      delete updated[fieldName]
      return updated
    })
  }, [])

  const clearAllErrors = useCallback(() => {
    setErrors({})
  }, [])

  const validateForm = useCallback(
    (data: Record<string, any>, schema: ValidationSchema): boolean => {
      const newErrors: ValidationErrors = {}
      let isValid = true

      for (const [fieldName, rules] of Object.entries(schema)) {
        const value = data[fieldName]

        for (const rule of rules) {
          let fieldValid = true

          switch (rule.type) {
            case 'required':
              fieldValid = validateRequired(value, fieldName)
              break
            case 'email':
              fieldValid = validateEmail(value, fieldName)
              break
            case 'positive':
              fieldValid = validatePositive(value, fieldName)
              break
            case 'percentage':
              fieldValid = validatePercentage(value, fieldName)
              break
            case 'minLength':
              fieldValid = validateMinLength(value, rule.minLength || 0, fieldName)
              break
            case 'maxLength':
              fieldValid = validateMaxLength(value, rule.maxLength || 255, fieldName)
              break
          }

          if (!fieldValid) {
            isValid = false
            if (!newErrors[fieldName]) {
              newErrors[fieldName] = []
            }
            if (rule.message) {
              newErrors[fieldName].push(rule.message)
            }
          }
        }
      }

      setErrors(newErrors)
      return isValid
    },
    [validateRequired, validateEmail, validatePositive, validatePercentage, validateMinLength, validateMaxLength]
  )

  return {
    errors,
    hasErrors,
    validateEmail,
    validateRequired,
    validatePositive,
    validatePercentage,
    validateMinLength,
    validateMaxLength,
    setError,
    clearError,
    clearAllErrors,
    validateForm,
  }
}

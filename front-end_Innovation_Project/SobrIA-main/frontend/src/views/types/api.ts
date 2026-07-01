export interface AuthResponse {
  token: string;
}

export interface RegisterRequest {
  email: string;
  password?: string; // Optionnel si ton backend le gère ainsi
  name: string;
  surname: string;
  companyName: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface EvaluationFilters {
  minCarbon?: number;
  maxCarbon?: number;
  aiScore?: string;
  startDate?: string;
  endDate?: string;
}
"use client";

import React, { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Mail, Lock, LogIn } from "lucide-react";
import { useRouter } from "next/navigation";
import { authService } from "@/services/api/auth.service";
import { useAuthStore } from "@/store/useAuthStore";

const loginSchema = z.object({
  email: z.string().email("Invalid email address"),
  password: z.string().min(6, "Password must be at least 6 characters"),
});

type LoginFormValues = z.infer<typeof loginSchema>;

export default function LoginPage() {
  const [error, setError] = useState<string | null>(null);
  const router = useRouter();
  const setAuth = useAuthStore((state) => state.setAuth);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginFormValues) => {
    setError(null);
    try {
      const response = await authService.login({
        Email: data.email,
        Password: data.password
      });
      if (response.success && response.data) {
        setAuth(
          response.data.user,
          response.data.accessToken,
          response.data.refreshToken
        );
        router.push("/dashboard");
      } else {
        setError(response.message || "Invalid credentials");
      }
    } catch (err: any) {
      setError(err.response?.data?.message || "An error occurred during login");
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center p-4 bg-gradient-to-br from-primary-50 to-white">
      <div className="max-w-md w-full glass-card p-8 space-y-8 animate-in fade-in zoom-in duration-500">
        <div className="text-center">
          <h1 className="text-3xl font-bold text-gray-900 tracking-tight">
            Welcome Back
          </h1>
          <p className="mt-2 text-sm text-gray-600">
            Sign in to your DallyWorkReport account
          </p>
        </div>

        {error && (
          <div className="bg-red-50 text-red-500 p-3 rounded-lg text-sm text-center border border-red-100 animate-bounce">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          <Input
            {...register("email")}
            label="Email Address"
            type="email"
            placeholder="admin@example.com"
            icon={<Mail className="w-5 h-5" />}
            error={errors.email?.message}
          />

          <Input
            {...register("password")}
            label="Password"
            type="password"
            placeholder="••••••••"
            icon={<Lock className="w-5 h-5" />}
            error={errors.password?.message}
          />

          <Button type="submit" className="w-full" isLoading={isSubmitting} size="lg">
            Sign In
            <LogIn className="w-5 h-5 ml-2" />
          </Button>
          
          <div className="text-center">
             <p className="text-sm text-gray-600">
                Don't have an account?{" "}
                <button type="button" onClick={() => router.push('/signup')} className="text-primary font-semibold hover:underline">
                    Sign up here
                </button>
             </p>
          </div>
        </form>
      </div>
    </div>
  );
}

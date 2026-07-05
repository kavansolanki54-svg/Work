"use client";

import React from "react";
import { AlertCircle, Trash2, X } from "lucide-react";
import { cn } from "@/utils/cn";
import { useConfirmStore } from "@/store/useConfirmStore";
import { Button } from "./Button";

export const ConfirmDialog = () => {
  const { isOpen, options, close } = useConfirmStore();

  if (!isOpen || !options) return null;

  const handleConfirm = () => {
    options.onConfirm();
    close();
  };

  const handleCancel = () => {
    if (options.onCancel) options.onCancel();
    close();
  };

  const isDanger = options.variant === "danger";

  return (
    <div className="fixed inset-0 z-[100] flex items-center justify-center p-4">
      <div 
        className="absolute inset-0 bg-slate-900/60 backdrop-blur-sm animate-in fade-in duration-300"
        onClick={handleCancel}
      />
      <div className="relative w-full max-w-md bg-white rounded-3xl shadow-2xl overflow-hidden animate-in zoom-in-95 slide-in-from-top-4 duration-300">
        <div className="p-8">
          <div className="flex items-start gap-6">
            <div className={cn(
              "p-4 rounded-2xl shrink-0",
              isDanger ? "bg-red-50 text-red-600" : "bg-blue-50 text-blue-600"
            )}>
              {isDanger ? <Trash2 className="w-8 h-8" /> : <AlertCircle className="w-8 h-8" />}
            </div>
            <div className="flex-1 space-y-2">
              <h3 className="text-xl font-black text-slate-900 tracking-tight leading-tight">
                {options.title}
              </h3>
              <p className="text-sm text-slate-500 font-medium leading-relaxed">
                {options.message}
              </p>
            </div>
          </div>

          <div className="mt-10 flex gap-3">
            <Button 
              variant="outline" 
              onClick={handleCancel}
              className="flex-1 h-12 rounded-2xl border-slate-200 text-slate-600 font-bold hover:bg-slate-50"
            >
              {options.cancelText || "Cancel"}
            </Button>
            <Button 
              onClick={handleConfirm}
              className={cn(
                "flex-1 h-12 rounded-2xl font-bold shadow-lg shadow-opacity-20 transition-all active:scale-95",
                isDanger 
                  ? "bg-red-600 hover:bg-red-700 text-white shadow-red-200" 
                  : "bg-slate-800 hover:bg-slate-900 text-white shadow-slate-200"
              )}
            >
              {options.confirmText || (isDanger ? "Delete" : "Confirm")}
            </Button>
          </div>
        </div>
        
        <button 
          onClick={handleCancel}
          className="absolute top-4 right-4 p-2 rounded-full hover:bg-slate-100 text-slate-400 hover:text-slate-900 transition-all"
        >
          <X className="w-4 h-4" />
        </button>
      </div>
    </div>
  );
};

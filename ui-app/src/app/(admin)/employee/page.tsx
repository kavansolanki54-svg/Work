"use client";

import React, { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { 
  UserPlus, 
  Search, 
  Edit, 
  Trash2, 
  Mail, 
  Phone, 
  UserCircle,
  Shield,
  Filter,
  CheckCircle2,
  XCircle,
  Lock,
  Eye
} from "lucide-react";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { Table } from "@/components/ui/Table";
import { employeeService, Employee } from "@/services/api/employee.service";
import { masterService } from "@/services/api/master.service";
import { roleService, Role } from "@/services/api/role.service";
import { useAuthStore } from "@/store/useAuthStore";
import { useConfirmStore } from "@/store/useConfirmStore";
import { cn } from "@/utils/cn";
import { toast } from "sonner";

const getErrorMessage = (err: any) => {
    if (err.response?.data?.errors) {
        return Object.values(err.response.data.errors).flat().join(", ");
    }
    return err.response?.data?.message || err.message || "An unexpected error occurred";
};

const employeeSchema = z.object({
  employeeCode: z.coerce.number().min(1, "Code is required"),
  firstName: z.string().min(2, "First name is required"),
  middleName: z.string().optional(),
  lastName: z.string().min(2, "Last name is required"),
  email: z.string().email("Invalid email address").optional().or(z.literal("")),
  mobileNo: z.string().optional(),
  roleMasterId: z.coerce.number().min(1, "Role is required"),
  genderId: z.coerce.number().min(1, "Gender is required"),
  passwords: z.string().min(6, "Password must be at least 6 characters"),
  isAllowLogin: z.boolean().default(true),
});

type EmployeeFormValues = z.infer<typeof employeeSchema>;

export default function EmployeeMasterPage() {
  const queryClient = useQueryClient();
  const user = useAuthStore((state) => state.user);
  const companyId = user?.companyId || 1;

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingEmployee, setEditingEmployee] = useState<Employee | null>(null);

  const { data: employees = [], isLoading } = useQuery({
    queryKey: ["employees", companyId],
    queryFn: () => employeeService.list(companyId),
    select: (res) => res.data || [],
  });

  const { data: roles = [] } = useQuery({
    queryKey: ["roles", companyId],
    queryFn: () => roleService.list(companyId),
    select: (res) => res.data || [],
  });

  const { data: genders = [] } = useQuery({
    queryKey: ["lookups", "Gender"],
    queryFn: () => masterService.getLookups("Gender"),
    select: (res) => res.data || [],
  });

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<EmployeeFormValues>({
    resolver: zodResolver(employeeSchema),
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => employeeService.delete(id),
    onSuccess: (res) => {
      queryClient.invalidateQueries({ queryKey: ["employees", companyId] });
      toast.success(res.message || "Employee deleted successfully");
    },
    onError: (err: any) => {
      toast.error(getErrorMessage(err));
    }
  });

  const mutation = useMutation({
    mutationFn: (data: EmployeeFormValues) => {
       const employeeName = `${data.firstName} ${data.lastName}`.trim();
       
       return employeeService.save({ 
         ...data, 
         companyId, 
         employeeId: editingEmployee?.employeeId || 0,
         employeeName,
         employeePhotoFile: null,
         activeStatus: 1 
       });
    },
    onSuccess: (res) => {
      queryClient.invalidateQueries({ queryKey: ["employees", companyId] });
      setIsModalOpen(false);
      reset();
      setEditingEmployee(null);
      toast.success(res.message || (editingEmployee ? "Updated successfully" : "Created successfully"));
    },
    onError: (err: any) => {
        toast.error(getErrorMessage(err));
    }
  });

  const confirm = useConfirmStore((state) => state.confirm);

  const handleEdit = (employee: Employee) => {
    setEditingEmployee(employee);
    reset({
      employeeCode: employee.employeeCode || 0,
      firstName: employee.firstName || "",
      middleName: employee.middleName || "",
      lastName: employee.lastName || "",
      email: employee.email || "",
      mobileNo: employee.mobileNo || "",
      roleMasterId: employee.roleMasterId || 1,
      genderId: employee.genderId || 3,
      passwords: employee.passwords || "123456",
      isAllowLogin: employee.isAllowLogin ?? true,
    });
    setIsModalOpen(true);
  };

  const handleOpenAdd = () => {
    setEditingEmployee(null);
    reset({
        employeeCode: 0,
        firstName: "",
        middleName: "",
        lastName: "",
        email: "",
        mobileNo: "",
        roleMasterId: 1,
        genderId: 3,
        passwords: "",
        isAllowLogin: true,
    });
    setIsModalOpen(true);
  };

  const [searchTerm, setSearchTerm] = useState("");
  const [filterRole, setFilterRole] = useState<number>(0);

  const filteredEmployees = React.useMemo(() => {
    return employees.filter(e => {
      const name = e?.employeeName || "";
      const email = e?.email || "";
      const role = roles.find((r: any) => r.roleMasterId === e.roleMasterId)?.roleName || "";
      const search = searchTerm.toLowerCase().trim();
      
      const matchesSearch = name.toLowerCase().includes(search) || 
                            email.toLowerCase().includes(search) ||
                            role.toLowerCase().includes(search);
                            
      const matchesRole = filterRole === 0 || e.roleMasterId === filterRole;
      
      return matchesSearch && matchesRole;
    });
  }, [employees, searchTerm, filterRole, roles]);

  const handleDelete = (id: number) => {
    confirm({
        title: "Delete Account?",
        message: "Are you sure you want to delete this employee? This action cannot be undone and will disable their dashboard access.",
        variant: "danger",
        confirmText: "Delete Account",
        onConfirm: () => deleteMutation.mutate(id)
    });
  };

  const columns = [
    {
      header: "Employee Details",
      accessor: (item: Employee) => (
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center text-primary font-bold">
            {item.employeeName?.[0] || "?"}
          </div>
          <div>
            <div className="font-bold text-gray-900 leading-tight">{item.employeeName}</div>
            <div className="text-[10px] text-gray-400 font-mono tracking-widest leading-none mt-1">ID: EMP-{item.employeeCode}</div>
          </div>
        </div>
      )
    },
    {
      header: "Contact",
      accessor: (item: Employee) => (
        <div className="space-y-1">
          <div className="flex items-center gap-2 text-xs text-gray-600">
            <Mail className="w-3.5 h-3.5 text-gray-400" />
            {item.email}
          </div>
          <div className="flex items-center gap-2 text-xs text-gray-600">
            <Phone className="w-3.5 h-3.5 text-gray-400" />
            {item.mobileNo || "N/A"}
          </div>
        </div>
      )
    },
    {
      header: "Role",
      accessor: (item: Employee) => {
        const role = roles.find((r: Role) => r.roleMasterId === item.roleMasterId);
        return (
          <span className="px-3 py-1 rounded-full bg-blue-50 text-blue-600 text-[10px] uppercase font-bold tracking-wider">
            {role?.roleName || "Member"}
          </span>
        );
      }
    },
    {
      header: "Allow Login",
      accessor: (item: Employee) => (
        <div className="flex items-center gap-2">
            {item.isAllowLogin ? (
                <>
                    <div className="w-6 h-6 rounded-lg bg-blue-50 flex items-center justify-center text-blue-500">
                      <Eye className="w-3.5 h-3.5" />
                    </div>
                    <span className="text-blue-600 font-bold text-[10px] uppercase tracking-wider">Allow</span>
                </>
            ) : (
                <>
                    <div className="w-6 h-6 rounded-lg bg-gray-50 flex items-center justify-center text-gray-400">
                      <Lock className="w-3.5 h-3.5" />
                    </div>
                    <span className="text-gray-400 font-bold text-[10px] uppercase tracking-wider">Denied</span>
                </>
            )}
        </div>
      )
    },
    {
      header: "Actions",
      accessor: (item: Employee) => (
        <div className="flex items-center gap-2 group-hover:opacity-100 transition-opacity">
          <button 
            onClick={() => handleEdit(item)}
            className="p-2 hover:bg-gray-100 rounded-lg text-gray-400 hover:text-primary transition-colors"
          >
            <Edit className="w-4 h-4" />
          </button>
          <button 
            onClick={() => handleDelete(item.employeeId)}
            className="p-2 hover:bg-red-50 rounded-lg text-gray-400 hover:text-red-500 transition-colors"
          >
            <Trash2 className="w-4 h-4" />
          </button>
        </div>
      )
    }
  ];

  return (
    <div className="space-y-8 animate-in fade-in duration-500">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 tracking-tight flex items-center gap-3">
             <UserCircle className="w-8 h-8 text-primary" />
             Employee Directory
          </h1>
          <p className="text-gray-500 mt-1 font-medium">Manage your team members and their roles.</p>
        </div>
        
        <Button onClick={handleOpenAdd} className="gap-2 px-6 shadow-xl shadow-primary/20">
          <UserPlus className="w-5 h-5" />
          Add Team Member
        </Button>
      </div>

      <div className="flex items-center gap-4 bg-white p-4 rounded-2xl shadow-sm border border-gray-100">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
          <input 
            type="text" 
            placeholder="Search by name or email..." 
            className="w-full bg-gray-50 border-none rounded-xl py-2.5 pl-10 pr-4 text-sm focus:ring-2 focus:ring-primary/20 transition-all outline-none"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>
        <div className="flex items-center gap-2 bg-gray-50 px-3 py-1 rounded-xl border border-gray-100 hover:shadow-sm transition-all focus-within:ring-2 focus-within:ring-primary/20">
          <Filter className="w-3.5 h-3.5 text-gray-400" />
          <select 
              className="bg-transparent border-none text-xs font-bold text-gray-500 cursor-pointer outline-none py-1.5"
              value={filterRole}
              onChange={(e) => setFilterRole(Number(e.target.value))}
          >
              <option value={0}>All Roles</option>
              {roles.map((r: Role) => (
                  <option key={r.roleMasterId} value={r.roleMasterId}>{r.roleName}</option>
              ))}
          </select>
        </div>
      </div>

      <Table 
        data={filteredEmployees} 
        columns={columns} 
        isLoading={isLoading} 
      />

      {/* Employee Modal */}
      <Modal 
        isOpen={isModalOpen} 
        onClose={() => setIsModalOpen(false)} 
        title={editingEmployee ? "Update Employee Details" : "Onboard New Team Member"}
        size="xl"
      >
        <form onSubmit={handleSubmit((data) => mutation.mutate(data))} className="space-y-4">
          <div className="bg-gray-50/50 p-3 rounded-xl border border-gray-100 space-y-3">
             <div className="grid grid-cols-1 md:grid-cols-12 gap-4 items-start">
                <div className="md:col-span-3">
                    <Input
                        {...register("employeeCode")}
                        label="Code"
                        placeholder="101"
                        error={errors.employeeCode?.message}
                    />
                </div>
                <div className="md:col-span-3">
                     <Input
                        {...register("firstName")}
                        label="First Name"
                        placeholder="John"
                        error={errors.firstName?.message}
                    />
                </div>
                <div className="md:col-span-3">
                     <Input
                        {...register("middleName")}
                        label="Middle"
                        placeholder="M."
                        error={errors.middleName?.message}
                    />
                </div>
                <div className="md:col-span-3">
                     <Input
                        {...register("lastName")}
                        label="Last Name"
                        placeholder="Doe"
                        error={errors.lastName?.message}
                    />
                </div>
             </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              <Input
                {...register("email")}
                label="Email Address"
                type="email"
                placeholder="john@company.com"
                error={errors.email?.message}
                icon={<Mail className="w-4 h-4" />}
              />
              <Input
                {...register("mobileNo")}
                label="Phone Number"
                placeholder="+1 234 567 890"
                error={errors.mobileNo?.message}
                icon={<Phone className="w-4 h-4" />}
              />
               <Input
                {...register("passwords")}
                label="Security Password"
                type="password"
                placeholder="••••••••"
                error={errors.passwords?.message}
                icon={<Lock className="w-4 h-4" />}
            />
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div className="space-y-1">
                <label className="text-[10px] font-bold text-gray-400 uppercase tracking-widest ml-1">Access Role</label>
                <div className="relative flex items-center group">
                    <Shield className="absolute left-3 w-4 h-4 text-gray-400" />
                    <select 
                        {...register("roleMasterId")}
                        className="w-full px-4 pl-10 py-2 bg-gray-50 border border-gray-100 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/20 text-sm font-medium transition-all"
                    >
                        {roles.map((r: Role) => <option key={r.roleMasterId} value={r.roleMasterId}>{r.roleName}</option>)}
                    </select>
                </div>
              </div>

              <div className="space-y-1">
                <label className="text-[10px] font-bold text-gray-400 uppercase tracking-widest ml-1">Gender Identity</label>
                <div className="relative flex items-center group">
                    <UserCircle className="absolute left-3 w-4 h-4 text-gray-400" />
                    <select 
                        {...register("genderId")}
                        className="w-full px-4 pl-10 py-2 bg-gray-50 border border-gray-100 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary/20 text-sm font-medium transition-all"
                    >
                        {genders.map(g => <option key={g.id} value={g.id}>{g.name}</option>)}
                    </select>
                </div>
              </div>

              <div className="flex items-center justify-between p-2.5 bg-gray-50/50 rounded-xl border border-dashed border-gray-200 h-[58px] mt-4">
                 <div className="flex flex-col">
                    <span className="text-[10px] font-bold text-gray-700 leading-none">ALLOW LOGIN</span>
                    <span className="text-[8px] text-gray-400 leading-none mt-0.5">App access</span>
                 </div>
                 <label className="relative inline-flex items-center cursor-pointer scale-90">
                    <input type="checkbox" {...register("isAllowLogin")} className="sr-only peer" />
                    <div className="w-9 h-5 bg-gray-200 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-4 after:w-4 after:transition-all peer-checked:bg-primary"></div>
                 </label>
              </div>
          </div>

          <div className="pt-4 flex items-center justify-end gap-3 border-t border-gray-50">
             <Button variant="outline" type="button" onClick={() => setIsModalOpen(false)} className="px-6 h-9 text-xs">Cancel</Button>
             <Button type="submit" isLoading={mutation.isPending} className="px-8 h-9 text-xs shadow-md">
                {editingEmployee ? "Update Personnel" : "Onboard Member"}
             </Button>
          </div>
        </form>
      </Modal>

    </div>
  );
}

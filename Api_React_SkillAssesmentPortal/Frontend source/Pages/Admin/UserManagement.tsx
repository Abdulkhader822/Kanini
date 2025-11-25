import { useEffect, useState } from "react";
import {
  Box, Button, Paper, Stack, TextField, Typography, Table, TableHead, TableRow,
  TableCell, TableBody, Select, MenuItem, TableContainer, FormControl, InputLabel
} from "@mui/material";
import { toast } from 'react-toastify';
import { http } from "../../Api/http";

interface User {
  userId: number;
  name: string;
  email: string;
  role: string;
}

export default function UserManagement() {
  const [users, setUsers] = useState<User[]>([]);
  const [searchId, setSearchId] = useState("");
  const [form, setForm] = useState({ name: "", email: "", password: "", role: "User" });
  const [editId, setEditId] = useState<number | null>(null);

  const fetchUsers = async () => {
    const { data } = await http.get<User[]>("/User");
    setUsers(data);
  };

  const handleAddOrUpdate = async () => {
    try {
      if (editId) {
        await http.put(`/User/${editId}`, form);
        toast.success("User updated successfully", { toastId: "user-updated" });
      } else {
        await http.post("/User", form);
        toast.success("User added successfully", { toastId: "user-added" });
      }
      setForm({ name: "", email: "", password: "", role: "User" });
      setEditId(null);
      fetchUsers();
    } catch (err: any) {
      // Error handled by interceptor
    }
  };

  const handleSearch = async () => {
    if (!searchId) return fetchUsers();
    try {
      const { data } = await http.get<User>(`/User/${searchId}`);
      setUsers([data]);
    } catch {
      toast.error("User not found", { toastId: "user-not-found" });
    }
  };

  const handleEdit = (user: User) => {
    setEditId(user.userId);
    setForm({ name: user.name, email: user.email, password: "", role: user.role });
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm("Are you sure you want to delete this user?")) return;
    
    try {
      await http.delete(`/User/${id}`);
      toast.success("User deleted successfully", { toastId: "user-deleted" });
      fetchUsers();
    } catch (err: any) {
      if (err.response?.status === 500) {
        toast.error("Cannot delete user. User may have associated data.", { toastId: "delete-error" });
      }
    }
  };

  useEffect(() => { fetchUsers(); }, []);

  return (
    <Box sx={{ maxWidth: 1000, mx: "auto", p: 3, background: "#fff", borderRadius: 3, boxShadow: 3 }}>
      <Typography variant="h6" fontWeight="bold" gutterBottom>
        👤 User Management
      </Typography>

      <Stack direction="row" spacing={2} sx={{ mb: 2 }}>
        <TextField
          label="Search by User ID"
          size="small"
          type="number"
          value={searchId}
          onChange={(e) => setSearchId(e.target.value)}
        />
        <Button variant="contained" onClick={handleSearch}>
          Search
        </Button>
        <Button variant="outlined" onClick={() => fetchUsers()}>
          Show All
        </Button>
      </Stack>

      <Stack direction={{ xs: "column", md: "row" }} spacing={2} sx={{ mb: 3 }}>
        <TextField
          label="Full Name"
          value={form.name}
          onChange={(e) => setForm({ ...form, name: e.target.value })}
          fullWidth
        />
        <TextField
          label="Email"
          value={form.email}
          onChange={(e) => setForm({ ...form, email: e.target.value })}
          fullWidth
        />
        <TextField
          label="Password"
          type="password"
          value={form.password}
          onChange={(e) => setForm({ ...form, password: e.target.value })}
          fullWidth
        />
        <FormControl fullWidth>
          <InputLabel>Role</InputLabel>
          <Select
            value={form.role}
            onChange={(e) => setForm({ ...form, role: e.target.value })}
            label="Role"
          >
            <MenuItem value="User">User</MenuItem>
            <MenuItem value="Admin">Admin</MenuItem>
          </Select>
        </FormControl>
        <Button variant="contained" onClick={handleAddOrUpdate} sx={{ whiteSpace: "nowrap" }}>
          {editId ? "Update" : "Add"}
        </Button>
        {editId && (
          <Button variant="outlined" onClick={() => { setEditId(null); setForm({ name: "", email: "", password: "", role: "User" }); }}>
            Cancel
          </Button>
        )}
      </Stack>

      <TableContainer component={Paper} sx={{ borderRadius: 2 }}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell><b>ID</b></TableCell>
              <TableCell><b>Name</b></TableCell>
              <TableCell><b>Email</b></TableCell>
              <TableCell><b>Role</b></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {users.map((u) => (
              <TableRow key={u.userId}>
                <TableCell>{u.userId}</TableCell>
                <TableCell>{u.name}</TableCell>
                <TableCell>{u.email}</TableCell>
                <TableCell>{u.role}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Box>
  );
}

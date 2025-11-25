const KEY = "auth_token";
const ROLE = "auth_role";
const USERNAME = "userName";

export const tokenstore = {
  // token
  get(): string | null {
    return localStorage.getItem(KEY);
  },
  set(token: string) {
    localStorage.setItem(KEY, token);
  },
  clear() {
    localStorage.removeItem(KEY);
    localStorage.removeItem(ROLE);
    localStorage.removeItem(USERNAME);
  },

  // role
  getRole(): string | null {
    return localStorage.getItem(ROLE);
  },
  setRole(role: string | null) {
    if (!role) {
      localStorage.removeItem(ROLE);
      return;
    }
    localStorage.setItem(ROLE, role);
  },

  // username
  getUserName(): string | null {
    return localStorage.getItem(USERNAME);
  },
  setUserName(name: string) {
    localStorage.setItem(USERNAME, name);
  },

  // userId from token
  getUserId(): number | null {
    const token = this.get();
    if (!token) return null;
    
    try {
      const decoded: any = JSON.parse(atob(token.split('.')[1]));
      const userId = decoded?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
      return userId ? Number(userId) : null;
    } catch {
      return null;
    }
  }
};

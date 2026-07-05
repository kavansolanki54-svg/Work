export const API_BASE_URL = "http://localhost:5083";
export const API_URL = `${API_BASE_URL}/api`;

/**
 * Helper to get the full URL for any resource (like images, logos, etc.)
 * @param path The relative path from the server root
 * @returns The full URL string
 */
export const getFullUrl = (path: string | null | undefined) => {
  if (!path) return "";
  if (path.startsWith("http")) return path;

  // Ensure we don't have double slashes
  const cleanPath = path.startsWith("/") ? path : `/${path}`;
  return `${API_BASE_URL}${cleanPath}`;
};

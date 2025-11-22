import axios from "axios";

const axiosSingleton = axios.create({
  baseURL: import.meta.env.VITE_APP_API_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

axiosSingleton.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      sessionStorage.removeItem("token");
    }

    return Promise.reject(error);
  }
);

axiosSingleton.interceptors.request.use(
  (response) => {
    response.headers.Authorization = `Bearer ${
      sessionStorage.getItem("token") || ""
    }`;

    return response;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default axiosSingleton;

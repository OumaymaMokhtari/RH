import axios from "axios";

const API_URL = "http://localhost:5148/api";


// const getAuthHeaders = () => {
//   const token = localStorage.getItem("token");
//   return {
//     headers: {
//       Authorization: `Bearer ${token}`,
//     },
//   };
// };

export const fetchConges = async () => {
  const response = await axios.get(`${API_URL}/conges`);
  console.log("Congés récupérés :", response.data);
  return response.data;
};

export const createConge = async (data) => {
  const response = await axios.post(`${API_URL}/conges`, data);
  return response.data;
};

export const calculateLeaveDuration = async (startDate, endDate) => {
  const response = await axios.post(`${API_URL}/conges/calculer-duree`, {
    dateDebut: startDate,
    dateFin: endDate,
  });
  return response.data;
};

export const deleteConge = async (id) => {
  try {
    const response = await axios.delete(`${API_URL}/conges/${id}`);
    return response.data;
  } catch (error) {
    console.error(
      "Erreur lors de la suppression :",
      id,
      error.response?.data || error.message
    );
    throw error;
  }
};

export const fetchCongesParEmploye = async (employeId) => {
  const response = await axios.get(`${API_URL}/conges/par-employe/${employeId}`);
  return response.data;
};

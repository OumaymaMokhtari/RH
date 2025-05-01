import face_recognition
import os
import pyodbc
from datetime import date
from datetime import datetime, time


DOSSIER_EMPLOYES = DOSSIER_EMPLOYES = r"C:\Users\oumayma\source\repos\ReconnaissanceAbsence\employes_faces"
DOSSIER_CAPTURE = DOSSIER_CAPTURE = r"C:\Users\oumayma\source\repos\ReconnaissanceAbsence\captures"
FICHIER_CAPTURE = os.path.join(DOSSIER_CAPTURE, "aujourdhui.jpg")

def charger_visages():
    encodings, ids = [], []
    for fichier in os.listdir(DOSSIER_EMPLOYES):
        if fichier.endswith(".jpg"):
            chemin = os.path.join(DOSSIER_EMPLOYES, fichier)
            image = face_recognition.load_image_file(chemin)
            enc = face_recognition.face_encodings(image)
            if enc:
                ids.append(int(os.path.splitext(fichier)[0]))
                encodings.append(enc[0])
    return encodings, ids

def reconnaitre(enc_connus, ids_connus):
    if not os.path.exists(FICHIER_CAPTURE):
        print("DEBUG_CAPTURE_MANQUANTE")
        return None

    image = face_recognition.load_image_file(FICHIER_CAPTURE)
    encs = face_recognition.face_encodings(image)
    if not encs:
        print("DEBUG_PAS_DE_VISAGE")
        return None

    matches = face_recognition.compare_faces(enc_connus, encs[0])
    if True in matches:
        id_trouve = ids_connus[matches.index(True)]
        return id_trouve

    print("absent:-1")
    return None

def enregistrer_absents(id_reconnu):
    try:
        conn = pyodbc.connect(
            "DRIVER={ODBC Driver 17 for SQL Server};"
            "SERVER=DESKTOP-1BFFPN3\\MSSQLSERVER2;"
            "DATABASE=GestionCongesDB;"
            "Trusted_Connection=yes;"
        )
        cursor = conn.cursor()
        cursor.execute("SELECT Id FROM Employes")
        all_ids = [row.Id for row in cursor.fetchall()]

        for emp_id in all_ids:
            if emp_id != id_reconnu:
                cursor.execute("""
                    SELECT COUNT(*) FROM Absences
                    WHERE EmployeId = ? AND DateAbsence = ?
                """, emp_id, date.today())

                if cursor.fetchone()[0] == 0:
                    cursor.execute("""
                        INSERT INTO Absences (EmployeId, DateAbsence, Raison)
                        VALUES (?, ?, ?)
                    """, emp_id, date.today(), "Retard")
                    print(f"retard:{emp_id}")

        conn.commit()
        conn.close()
    except Exception as e:
        print(f"ERROR_DB: {e}")

# === MAIN ===
if __name__ == "__main__":
    try:
        heure_actuelle = datetime.now().time()

        # Créneaux limites
        matin_limite = time(8, 30)
        apresmidi_limite = time(15, 0)

        enc_connus, ids_connus = charger_visages()
        id_reconnu = reconnaitre(enc_connus, ids_connus)

        if id_reconnu is not None:
            if heure_actuelle <= matin_limite or heure_actuelle <= apresmidi_limite:
                print(f"present:{id_reconnu}")  # marqué présent
            else:
                enregistrer_absents(id_reconnu)
                print(f"absent_heure:{id_reconnu}")
        else:
            print("absent:-1")  
    except Exception as ex:
        print(f"error_main:{ex}")

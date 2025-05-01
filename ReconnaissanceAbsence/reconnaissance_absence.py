import cv2
import face_recognition
import os
import pyodbc
from datetime import date

# ========== CONFIGURATION ==========
DOSSIER_EMPLOYES = "employes_faces"
DOSSIER_CAPTURE = "captures"
FICHIER_CAPTURE = os.path.join(DOSSIER_CAPTURE, "aujourdhui.jpg")

# Créer dossier si nécessaire
os.makedirs(DOSSIER_CAPTURE, exist_ok=True)
# ========== ÉTAPE 1 : CAPTURE WEBCAM ==========
def capturer_image():
    camera = cv2.VideoCapture(0)
    if not camera.isOpened():
        print("Erreur: webcam non accessible")
        return False

    print("Appuie sur 'c' pour capturer, 'q' pour quitter.")
    while True:
        ret, frame = camera.read()
        if not ret:
            print("Erreur lecture webcam.")
            break

        cv2.imshow("Caméra - Reconnaissance", frame)
        key = cv2.waitKey(1) & 0xFF

        if key == ord("c"):
            cv2.imwrite(FICHIER_CAPTURE, frame)
            break
        elif key == ord("q"):
            break

    camera.release()
    cv2.destroyAllWindows()
    return os.path.exists(FICHIER_CAPTURE)

# ========== ÉTAPE 2 : RECONNAISSANCE ==========
def charger_visages():
    encodings = []
    ids = []
    for fichier in os.listdir(DOSSIER_EMPLOYES):
        if fichier.endswith(".jpg"):
            chemin = os.path.join(DOSSIER_EMPLOYES, fichier)
            image = face_recognition.load_image_file(chemin)
            enc = face_recognition.face_encodings(image)
            if enc:
                employe_id = int(os.path.splitext(fichier)[0])
                encodings.append(enc[0])
                ids.append(employe_id)
    return encodings, ids

def reconnaitre(enc_connus, ids_connus):
    image = face_recognition.load_image_file(FICHIER_CAPTURE)
    encs = face_recognition.face_encodings(image)
    if not encs:
        print("absent:-1")  # Aucun visage détecté
        return None
    matches = face_recognition.compare_faces(enc_connus, encs[0], tolerance=0.6)
    if True in matches:
        index = matches.index(True)
        id_trouve = ids_connus[index]
        print(f"present:{id_trouve}")  # Pour .NET
        return id_trouve
    print("absent:-1")  # Visage non reconnu
    return None

# ========== ÉTAPE 3 : ENREGISTREMENT ==========
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
                    print(f"retard:{emp_id}")  # Pour .NET
        conn.commit()
        conn.close()
    except Exception as e:
        print("Erreur DB:", e)

# ========== MAIN ==========
if __name__ == "__main__":
    if capturer_image():
        enc_connus, ids_connus = charger_visages()
        id_reconnu = reconnaitre(enc_connus, ids_connus)
        if id_reconnu is not None:
            enregistrer_absents(id_reconnu)

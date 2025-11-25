package com.soc.testwsclient;

import com.soap.ws.client.generated.*;
import jakarta.xml.bind.JAXBElement;
import java.util.Scanner;

public class App {
    public static void main(String[] args) {
        System.out.println("==========================================");
        System.out.println("  CLIENT LOURD SOAP - LETS GO BIKING  ");
        System.out.println("==========================================");

        try {
            IRoutingSoapService_Service service = new IRoutingSoapService_Service();
            IRoutingSoapService proxy = service.getBasicHttpBindingIRoutingSoapService();
            Scanner scanner = new Scanner(System.in);

            while (true) {
                System.out.println("\n------------------------------------------------");
                System.out.println("Entrez votre point de départ (ou 'q' pour quitter) :");
                String depart = scanner.nextLine();
                if (depart.equalsIgnoreCase("q")) break;

                System.out.println("Entrez votre destination :");
                String arrivee = scanner.nextLine();

                System.out.println(" Appel du serveur SOAP C# en cours...");

                RouteLeg resultat = proxy.getItinerary(depart, arrivee);

                System.out.println("\n Réponse reçue !");
                System.out.println("Temps total : " + String.format("%.1f", resultat.getTotalSeconds() / 60) + " minutes");

                System.out.println("\n--- Description ---");
                if (resultat.getDescription() != null) {
                    System.out.println(resultat.getDescription().getValue());
                } else {
                    System.out.println("(Aucune description disponible)");
                }

            }
            scanner.close();
            System.out.println("Au revoir !");

        } catch (Exception e) {
            System.err.println("\n ERREUR :");
            e.printStackTrace();
        }
    }
}
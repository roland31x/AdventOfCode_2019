package aoc_api;
import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.util.List;

public class InputGetter{

    private void CacheInput(int day, int year){
        String url = "https://adventofcode.com/" + year + "/day/" + day + "/input";
        String sesh = null;
        try{
            File session = new File("session.txt");
            BufferedReader reader = new BufferedReader(new FileReader(session));
            List<String> lines = reader.lines().toList();
            reader.close();
            sesh = lines.getFirst();
        }
        catch (Exception e){
            System.out.println("Error reading session.txt");
        }

        if(sesh == null){
            System.out.println("No session cookie found");
            return;
        }

        String cookie = "session=" + sesh;

        try{
            ProcessBuilder pb = new ProcessBuilder("curl", url, "-H", "Cookie: " + cookie, "-o", "inputs/input" + day + ".txt");
            pb.start();
        }
        catch (Exception e){
            System.out.println("Error fetching input");
        }

    }

    public List<String> getInput(int day, int year){

        File dir = new File("inputs");
        if(!dir.exists()){
            dir.mkdir();
        }

        String path = "inputs/input" + day + ".txt";
        File input = new File(path);
        if(!input.exists()){
            CacheInput(day, year);
        }

        try{
            BufferedReader reader = new BufferedReader(new FileReader(input));
            List<String> lines = reader.lines().toList();
            if(lines.getLast().isEmpty()){
                lines.removeLast();
            }
            reader.close();
            return lines;
        }
        catch (Exception e){
            System.out.println("Error reading input");
            return null;
        }

    }

    public List<String> getInput(int day){
        return getInput(day, 2019);
    }
}



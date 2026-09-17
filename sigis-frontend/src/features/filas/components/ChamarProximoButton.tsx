import { PhoneCall } from "lucide-react";
import { Button } from "@/components/ui/button";

interface ChamarProximoButtonProps {
  onClick: () => void;
  disabled?: boolean;
}

export function ChamarProximoButton({ onClick, disabled }: ChamarProximoButtonProps) {
  return (
    <Button className="bg-sus-blue hover:bg-sus-blue-dark" onClick={onClick} disabled={disabled}>
      <PhoneCall className="size-4" />
      Chamar proximo
    </Button>
  );
}
